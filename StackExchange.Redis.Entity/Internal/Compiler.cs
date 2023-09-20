using System.Linq.Expressions;
using System.Reflection;

namespace StackExchange.Redis.Entity.Internal;

internal static class Compiler
{
    private static readonly MethodInfo MethodDeserialize = typeof(RedisValueDeserializerProxy).GetMethod(nameof(RedisValueDeserializerProxy.Deserialize))!;
    private static readonly MethodInfo MethodSerialize = typeof(IRedisValueSerializer).GetMethod(nameof(IRedisValueSerializer.Serialize))!;

    private static readonly ParameterExpression ParameterRedisValue = Expression.Parameter(typeof(RedisValue), "redisValue");
    private static readonly ParameterExpression ParameterDeserializer = Expression.Parameter(typeof(RedisValueDeserializerProxy), "deserializer");
    private static readonly ParameterExpression ParameterSerializer = Expression.Parameter(typeof(IRedisValueSerializer), "serializer");

    /*
     Expression<Func<Document, IRedisValueSerializer, RedisValue>> exp =
            (Document entity, IRedisValueSerializer serializer) => serializer.Serialize(entity.Name);
     */
    internal static RedisValueReader<T> GetReader<T>(PropertyInfo property)
    {
        var pEntity = Expression.Parameter(typeof(T), "entity");

        var eProperty = Expression.Property(pEntity, property);

        var eCall = Expression.Call(ParameterSerializer, MethodSerialize.MakeGenericMethod(property.PropertyType), eProperty);

        var lambda = Expression.Lambda<RedisValueReader<T>>(eCall, pEntity, ParameterSerializer);

        return lambda.Compile();
    }

    /*
     Expression<Func<Document, RedisValue, RedisValueDeserializerProxy, string?>> exp =
            (Document entity, RedisValue redisValue, RedisValueDeserializerProxy deserializer)
            => deserializer.Deserialize(in redisValue, entity.Name);
     */
    internal static RedisValueWriter<T> GetWriter<T>(PropertyInfo property)
    {
        var pEntity = Expression.Parameter(typeof(T), "entity");

        var eProperty = Expression.Property(pEntity, property);

        var eCall = Expression.Call(ParameterDeserializer, MethodDeserialize.MakeGenericMethod(property.PropertyType), ParameterRedisValue, eProperty);

        var eAssign = Expression.Assign(eProperty, eCall);

        var lambda = Expression.Lambda<RedisValueWriter<T>>(eAssign, pEntity, ParameterRedisValue, ParameterDeserializer);

        return lambda.Compile();
    }
}