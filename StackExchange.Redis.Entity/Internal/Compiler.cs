﻿using System.Linq.Expressions;
using System.Reflection;

namespace StackExchange.Redis.Entity.Internal;

internal static class Compiler
{
    private static readonly Type RedisValueType = typeof(RedisValue);
    private static readonly MethodInfo MethodDeserialize = typeof(RedisValueDeserializerProxy).GetMethod(nameof(RedisValueDeserializerProxy.Deserialize))!;
    private static readonly MethodInfo MethodSerialize = typeof(IRedisValueSerializer).GetMethod(nameof(IRedisValueSerializer.Serialize))!;

    private static readonly ParameterExpression ParameterRedisValue = Expression.Parameter(typeof(RedisValue), "redisValue");
    private static readonly ParameterExpression ParameterDeserializer = Expression.Parameter(typeof(RedisValueDeserializerProxy), "deserializer");
    private static readonly ParameterExpression ParameterSerializer = Expression.Parameter(typeof(IRedisValueSerializer), "serializer");

    /*
     Expression<Func<Document, IRedisValueSerializer, RedisValue>> exp =
            (Document entity, IRedisValueSerializer serializer) => serializer.Serialize(entity.Name);
     */
    internal static RedisValueReader<T> CompileReader<T>(PropertyInfo property)
    {
        var pEntity = Expression.Parameter(typeof(T), "entity");

        Expression eProperty = Expression.Property(pEntity, property);

        var propertyType = property.PropertyType;

        Expression eBody;

        if (propertyType.Equals(RedisValueType))
        {
            eBody = eProperty;
        }
        else
        {
            if (propertyType.IsEnum)
            {
                propertyType = propertyType.GetEnumUnderlyingType();
                eProperty = Expression.ConvertChecked(eProperty, propertyType);
            }

            eBody = Expression.Call(ParameterSerializer, MethodSerialize.MakeGenericMethod(propertyType), eProperty);
        }

        var lambda = Expression.Lambda<RedisValueReader<T>>(eBody, pEntity, ParameterSerializer);

        return lambda.Compile();
    }

    /*
     Expression<Func<Document, RedisValue, RedisValueDeserializerProxy, string?>> exp =
            (Document entity, RedisValue redisValue, RedisValueDeserializerProxy deserializer)
            => deserializer.Deserialize(in redisValue, entity.Name);
     */
    internal static RedisValueWriter<T> CompileWriter<T>(PropertyInfo property)
    {
        var pEntity = Expression.Parameter(typeof(T), "entity");

        var eProperty = Expression.Property(pEntity, property);

        var propertyType = property.PropertyType;

        Expression eRight;

        if (propertyType.IsEnum)
        {
            var enumType = propertyType.GetEnumUnderlyingType();

            var eConvert = Expression.ConvertChecked(eProperty, enumType);

            var call = Expression.Call(ParameterDeserializer, MethodDeserialize.MakeGenericMethod(enumType), ParameterRedisValue, eConvert);

            eRight = Expression.ConvertChecked(call, propertyType);
        }
        else if (propertyType.Equals(RedisValueType))
        {
            eRight = ParameterRedisValue;
        }
        else
        {
            eRight = Expression.Call(ParameterDeserializer, MethodDeserialize.MakeGenericMethod(propertyType), ParameterRedisValue, eProperty);
        }

        var eAssign = Expression.Assign(eProperty, eRight);

        var lambda = Expression.Lambda<RedisValueWriter<T>>(eAssign, pEntity, ParameterRedisValue, ParameterDeserializer);

        return lambda.Compile();
    }
}