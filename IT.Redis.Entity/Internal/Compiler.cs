using System.Linq.Expressions;
using System.Reflection;

namespace IT.Redis.Entity.Internal;

internal static class Compiler
{
    private static readonly string FieldRedisKeyBitsName = "_redisKeyBits";
    internal static readonly string PropRedisKeyBitsName = "RedisKeyBits";
    private static readonly Type RedisKeyBitsType = typeof(byte);

    private static readonly string FieldRedisKeyName = "_redisKey";
    internal static readonly string PropRedisKeyName = "RedisKey";
    private static readonly Type RedisKeyType = typeof(byte[]);

    private static readonly MethodInfo MethodDeserializeNew = typeof(RedisValueDeserializerProxy).GetMethod(nameof(RedisValueDeserializerProxy.DeserializeNew))!;
    private static readonly MethodInfo MethodDeserialize = typeof(RedisValueDeserializerProxy).GetMethod(nameof(RedisValueDeserializerProxy.Deserialize))!;
    private static readonly MethodInfo MethodSerialize = typeof(IRedisValueSerializer).GetMethod(nameof(IRedisValueSerializer.Serialize))!;

    private static readonly ParameterExpression ParameterRedisValue = Expression.Parameter(typeof(RedisValue), "redisValue");
    private static readonly ParameterExpression ParameterDeserializer = Expression.Parameter(typeof(RedisValueDeserializerProxy), "deserializer");
    private static readonly ParameterExpression ParameterSerializer = Expression.Parameter(typeof(IRedisValueSerializer), "serializer");
    private static readonly ParameterExpression ParameterKeyBuilder = Expression.Parameter(typeof(EntityKeyBuilder), "keyBuilder");

    /*
     Expression<Func<Document, IRedisValueSerializer, RedisValue>> exp =
            (Document entity, IRedisValueSerializer serializer) => serializer.Serialize(entity.Name);
     */
    internal static RedisValueReader<T> GetReader<T>(PropertyInfo property)
    {
        var eEntity = Expression.Parameter(typeof(T), "entity");

        var eProperty = Expression.Property(eEntity, property);

        var eCall = Expression.Call(ParameterSerializer, MethodSerialize.MakeGenericMethod(property.PropertyType), eProperty);

        var lambda = Expression.Lambda<RedisValueReader<T>>(eCall, eEntity, ParameterSerializer);

        return lambda.Compile();
    }

    /*
     Expression<Func<Document, RedisValue, RedisValueDeserializerProxy, string?>> exp =
            (Document entity, RedisValue redisValue, RedisValueDeserializerProxy deserializer)
            => deserializer.Deserialize(in redisValue, entity.Name);
     */
    internal static RedisValueWriter<T> GetWriter<T>(PropertyInfo property)
    {
        var eEntity = Expression.Parameter(typeof(T), "entity");

        var eProperty = Expression.Property(eEntity, property);

        var eCall = property.GetMethod == null 
            ? Expression.Call(ParameterDeserializer, MethodDeserializeNew.MakeGenericMethod(property.PropertyType), ParameterRedisValue)
            : Expression.Call(ParameterDeserializer, MethodDeserialize.MakeGenericMethod(property.PropertyType), ParameterRedisValue, eProperty);

        var eAssign = Expression.Assign(eProperty, eCall);

        var lambda = Expression.Lambda<RedisValueWriter<T>>(eAssign, eEntity, ParameterRedisValue, ParameterDeserializer);

        return lambda.Compile();
    }

    //keyBuilder.Build(entity.Key1, entity.Key2)
    internal static Func<T, EntityKeyBuilder, byte[]> GetReaderKey<T>(IReadOnlyList<PropertyInfo> keys)
    {
        var entityType = typeof(T);

        var eEntity = Expression.Parameter(entityType, "entity");

        var eRedisKey = GetRedisKey(eEntity, entityType);

        var propertyTypes = new Type[keys.Count];
        var eArguments = new Expression[2 + keys.Count];

        var hasKeySetter = false;
        var eNullBytes = Expression.Constant(null, RedisKeyType);
        var eZeroByte = Expression.Constant((byte)0, RedisKeyBitsType);

        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            propertyTypes[i] = key.PropertyType;
            eArguments[i + 2] = Expression.Property(eEntity, key);

            if (key.SetMethod != null) hasKeySetter = true;
        }

        var methodBuild = GetMethodBuild(keys.Count).MakeGenericMethod(propertyTypes);

        Expression eBody;

        if (hasKeySetter)
        {
            var eRedisKeyBits = GetRedisKeyBits(eEntity, entityType);
            
            var eTest = Expression.Or(
                Expression.GreaterThan(eRedisKeyBits, eZeroByte),
                Expression.Equal(eRedisKey, eNullBytes));

            eArguments[0] = eRedisKey;
            eArguments[1] = eRedisKeyBits;

            var eCall = Expression.Call(ParameterKeyBuilder, methodBuild, eArguments);

            var eAssign = Expression.Assign(eRedisKey, eCall);

            var eIfThen = Expression.IfThen(eTest, Expression.Block(
                    eAssign,
                    Expression.Assign(eRedisKeyBits, eZeroByte)
                ));

            eBody = Expression.Block(eIfThen, eRedisKey);
        }
        else
        {
            eArguments[0] = eNullBytes;
            eArguments[1] = eZeroByte;

            var eCall = Expression.Call(ParameterKeyBuilder, methodBuild, eArguments);

            var eAssign = Expression.Assign(eRedisKey, eCall);

            eBody = Expression.Coalesce(eRedisKey, eAssign);
        }

        var lambda = Expression.Lambda<Func<T, EntityKeyBuilder, byte[]>>(eBody, eEntity, ParameterKeyBuilder);

        return lambda.Compile();
    }

    private static Expression GetRedisKeyBits(ParameterExpression eEntity, Type entityType)
        => entityType.IsInterface
            ? Expression.Property(eEntity, GetProperty(entityType, PropRedisKeyBitsName, RedisKeyBitsType))
            : Expression.Field(eEntity, GetField(entityType, FieldRedisKeyBitsName, RedisKeyBitsType));


    private static Expression GetRedisKey(ParameterExpression eEntity, Type entityType)
        => entityType.IsInterface 
            ? Expression.Property(eEntity, GetProperty(entityType, PropRedisKeyName, RedisKeyType))
            : Expression.Field(eEntity, GetField(entityType, FieldRedisKeyName, RedisKeyType));

    private static PropertyInfo GetProperty(Type entityType, string propName, Type propType)
    {
        var property = entityType.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);

        if (property == null || property.PropertyType != propType || property.GetMethod == null || property.SetMethod == null)
            throw new InvalidOperationException($"Entity type '{entityType.FullName}' does not contain public property '{propName}' with type '{propType.FullName}' and get/set methods");

        return property;
    }

    private static FieldInfo GetField(Type entityType, string fieldName, Type fieldType)
    {
        var field = entityType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        if (field == null || field.FieldType != fieldType)
            throw new InvalidOperationException($"Entity type '{entityType.FullName}' does not contain non-public field '{fieldName}' with type '{fieldType.FullName}'");

        return field;
    }

    private static MethodInfo GetMethodBuild(int count)
    {
        var methods = typeof(EntityKeyBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public);
        for (int i = 0; i < methods.Length; i++)
        {
            var method = methods[i];
            if (method.Name == nameof(EntityKeyBuilder.BuildKey) && method.ContainsGenericParameters)
            {
                var args = method.GetGenericArguments();
                if (args.Length == count) return method;
            }
        }

        throw new InvalidOperationException($"Method '{nameof(EntityKeyBuilder.BuildKey)}' with {count} arguments not found");
    }
}