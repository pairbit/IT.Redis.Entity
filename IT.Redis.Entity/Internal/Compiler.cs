using System.Linq.Expressions;
using System.Reflection;

namespace IT.Redis.Entity.Internal;

internal static class Compiler
{
    private static readonly string FieldRedisKeyName = "_redisKey";
    private static readonly Type FieldRedisKeyType = typeof(byte[]);
    private static readonly BindingFlags FieldRedisKeyBinding = 
        BindingFlags.Instance | BindingFlags.NonPublic;

    private static readonly MethodInfo MethodDeserialize = typeof(RedisValueDeserializerProxy).GetMethod(nameof(RedisValueDeserializerProxy.Deserialize))!;
    private static readonly MethodInfo MethodSerialize = typeof(IRedisValueSerializer).GetMethod(nameof(IRedisValueSerializer.Serialize))!;

    private static readonly ParameterExpression ParameterRedisValue = Expression.Parameter(typeof(RedisValue), "redisValue");
    private static readonly ParameterExpression ParameterDeserializer = Expression.Parameter(typeof(RedisValueDeserializerProxy), "deserializer");
    private static readonly ParameterExpression ParameterSerializer = Expression.Parameter(typeof(IRedisValueSerializer), "serializer");
    private static readonly ParameterExpression ParameterKeyBuilder = Expression.Parameter(typeof(KeyBuilder), "keyBuilder");

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
    internal static Func<T, KeyBuilder, byte[]> GetReaderKey<T>(IReadOnlyList<PropertyInfo> keys)
    {
        var entityType = typeof(T);

        var eEntity = Expression.Parameter(entityType, "entity");

        var eField = Expression.Field(eEntity, GetFieldRedisKey(entityType));

        var propertyTypes = new Type[keys.Count];
        var properties = new Expression[keys.Count];

        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            propertyTypes[i] = key.PropertyType;
            properties[i] = Expression.Property(eEntity, key);
        }

        var methodBuild = GetMethodBuild(keys.Count);

        var eCall = Expression.Call(ParameterKeyBuilder, methodBuild.MakeGenericMethod(propertyTypes), properties);

        var eAssign = Expression.Assign(eField, eCall);

        var eCoalesce = Expression.Coalesce(eField, eAssign);

        var lambda = Expression.Lambda<Func<T, KeyBuilder, byte[]>>(eCoalesce, eEntity, ParameterKeyBuilder);

        return lambda.Compile();
    }

    private static FieldInfo GetFieldRedisKey(Type type)
    {
        var field = type.GetField(FieldRedisKeyName, FieldRedisKeyBinding);

        if (field == null || field.FieldType != FieldRedisKeyType)
            throw new InvalidOperationException($"Entity type '{type.FullName}' does not contain field '{FieldRedisKeyName}' with type '{FieldRedisKeyType.FullName}'");

        return field;
    }

    private static MethodInfo GetMethodBuild(int count)
    {
        var methods = typeof(KeyBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public);
        for (int i = 0; i < methods.Length; i++)
        {
            var method = methods[i];
            if (method.Name == nameof(KeyBuilder.Build) && method.ContainsGenericParameters)
            {
                var args = method.GetGenericArguments();
                if (args.Length == count) return method;
            }
        }

        throw new InvalidOperationException($"Method '{nameof(KeyBuilder.Build)}' with {count} arguments not found");
    }
}