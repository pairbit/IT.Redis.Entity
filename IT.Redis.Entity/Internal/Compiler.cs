using System.Linq.Expressions;
using System.Reflection;

namespace IT.Redis.Entity.Internal;

internal static class Compiler
{
    private static readonly string FieldRedisKeyBitsName = "_redisKeyBits";
    private static readonly Type FieldRedisKeyBitsType = typeof(byte);

    private static readonly string FieldRedisKeyName = "_redisKey";
    private static readonly Type FieldRedisKeyType = typeof(byte[]);
    private static readonly BindingFlags FieldRedisKeyBinding =
        BindingFlags.Instance | BindingFlags.NonPublic;

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

        var eCall = Expression.Call(ParameterDeserializer, MethodDeserialize.MakeGenericMethod(property.PropertyType), ParameterRedisValue, eProperty);

        var eAssign = Expression.Assign(eProperty, eCall);

        var lambda = Expression.Lambda<RedisValueWriter<T>>(eAssign, eEntity, ParameterRedisValue, ParameterDeserializer);

        return lambda.Compile();
    }

    //keyBuilder.Build(entity.Key1, entity.Key2)
    internal static Func<T, EntityKeyBuilder, byte[]> GetReaderKey<T>(IReadOnlyList<PropertyInfo> keys)
    {
        var entityType = typeof(T);

        var eEntity = Expression.Parameter(entityType, "entity");

        var eFieldRedisKey = Expression.Field(eEntity,
            GetField(entityType, FieldRedisKeyName, FieldRedisKeyType));

        var propertyTypes = new Type[keys.Count];
        var eArguments = new Expression[2 + keys.Count];

        var hasKeySetter = false;
        var eNullBytes = Expression.Constant(null, FieldRedisKeyType);
        var eZeroByte = Expression.Constant((byte)0, FieldRedisKeyBitsType);

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
            var eFieldBits = Expression.Field(eEntity, GetField(entityType, FieldRedisKeyBitsName, FieldRedisKeyBitsType));
            
            var eTest = Expression.Or(
                Expression.GreaterThan(eFieldBits, eZeroByte),
                Expression.Equal(eFieldRedisKey, eNullBytes));

            eArguments[0] = eFieldRedisKey;
            eArguments[1] = eFieldBits;

            var eCall = Expression.Call(ParameterKeyBuilder, methodBuild, eArguments);

            var eAssign = Expression.Assign(eFieldRedisKey, eCall);

            var eIfThen = Expression.IfThen(eTest, Expression.Block(
                    eAssign,
                    Expression.Assign(eFieldBits, eZeroByte)
                ));

            eBody = Expression.Block(eIfThen, eFieldRedisKey);
        }
        else
        {
            eArguments[0] = eNullBytes;
            eArguments[1] = eZeroByte;

            var eCall = Expression.Call(ParameterKeyBuilder, methodBuild, eArguments);

            var eAssign = Expression.Assign(eFieldRedisKey, eCall);

            eBody = Expression.Coalesce(eFieldRedisKey, eAssign);
        }

        var lambda = Expression.Lambda<Func<T, EntityKeyBuilder, byte[]>>(eBody, eEntity, ParameterKeyBuilder);

        return lambda.Compile();
    }

    private static FieldInfo GetField(Type entityType, string fieldName, Type fieldType)
    {
        var field = entityType.GetField(fieldName, FieldRedisKeyBinding);

        if (field == null || field.FieldType != fieldType)
            throw new InvalidOperationException($"Entity type '{entityType.FullName}' does not contain field '{fieldName}' with type '{fieldType.FullName}'");

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