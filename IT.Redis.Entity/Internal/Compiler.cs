using System.Linq.Expressions;
using System.Reflection;

namespace IT.Redis.Entity.Internal;

internal static class Compiler
{
    private static readonly string FieldNameRedisKey = "_redisKey";
    private static readonly string FieldNameRedisKeyBits = "_redisKeyBits";

    internal static readonly string PropNameRedisKey = "RedisKey";
    internal static readonly string PropNameRedisKeyBits = "RedisKeyBits";

    private static readonly Type TypeRedisKey = typeof(byte[]);
    private static readonly Type TypeRedisKeyBits = typeof(byte);
    private static readonly Type TypeKeyBuilder = typeof(IKeyBuilder);
    private static readonly Type TypeKeyRebuilder = typeof(IKeyRebuilder);

    private static readonly ConstantExpression NullRedisKey = Expression.Constant(null, TypeRedisKey);
    private static readonly ConstantExpression ZeroRedisKeyBits = Expression.Constant((byte)0, TypeRedisKeyBits);

    private static readonly MethodInfo MethodDeserializeNew = typeof(RedisValueDeserializerProxy).GetMethod(nameof(RedisValueDeserializerProxy.DeserializeNew))!;
    private static readonly MethodInfo MethodDeserialize = typeof(RedisValueDeserializerProxy).GetMethod(nameof(RedisValueDeserializerProxy.Deserialize))!;
    private static readonly MethodInfo MethodSerialize = typeof(IRedisValueSerializer).GetMethod(nameof(IRedisValueSerializer.Serialize))!;

    private static readonly ParameterExpression ParameterRedisValue = Expression.Parameter(typeof(RedisValue), "redisValue");
    private static readonly ParameterExpression ParameterDeserializer = Expression.Parameter(typeof(RedisValueDeserializerProxy), "deserializer");
    private static readonly ParameterExpression ParameterSerializer = Expression.Parameter(typeof(IRedisValueSerializer), "serializer");
    private static readonly ParameterExpression ParameterKeyRebuilder = Expression.Parameter(TypeKeyRebuilder, "keyRebuilder");

    /*
     Expression<Func<Document, IRedisValueSerializer, RedisValue>> exp =
            (Document entity, IRedisValueSerializer serializer) => serializer.Serialize(entity.Name);
     */
    internal static RedisValueReader<TEntity>? GetReader<TEntity>(PropertyInfo property)
    {
        if (property.GetMethod == null) return null;

        var eEntity = Expression.Parameter(typeof(TEntity), "entity");

        return Expression.Lambda<RedisValueReader<TEntity>>(
            Expression.Call(ParameterSerializer,
                MethodSerialize.MakeGenericMethod(property.PropertyType),
                Expression.Property(eEntity, property)),
            eEntity, ParameterSerializer).Compile();
    }

    /*
     Expression<Func<Document, RedisValue, RedisValueDeserializerProxy, string?>> exp =
            (Document entity, RedisValue redisValue, RedisValueDeserializerProxy deserializer)
            => entity.Name = deserializer.Deserialize(in redisValue, entity.Name);
     */
    internal static RedisValueWriter<TEntity>? GetWriter<TEntity>(PropertyInfo property)
    {
        if (property.SetMethod == null) return null;

        var eEntity = Expression.Parameter(typeof(TEntity), "entity");
        var eProperty = Expression.Property(eEntity, property);

        return Expression.Lambda<RedisValueWriter<TEntity>>(
            Expression.Assign(eProperty,
                property.GetMethod == null
                ? Expression.Call(ParameterDeserializer, MethodDeserializeNew.MakeGenericMethod(property.PropertyType), ParameterRedisValue)
                : Expression.Call(ParameterDeserializer, MethodDeserialize.MakeGenericMethod(property.PropertyType), ParameterRedisValue, eProperty)),
            eEntity, ParameterRedisValue, ParameterDeserializer).Compile();
    }

    //keyBuilder.Build(entity.Key1, entity.Key2)
    internal static KeyReader<TEntity>? GetKeyReader<TEntity>(IReadOnlyList<PropertyInfo> keys)
    {
        if (keys.Count == 0) return null;

        var entityType = typeof(TEntity);
        var eEntity = Expression.Parameter(entityType, "entity");
        var eRedisKey = GetPropOrField(eEntity, entityType, TypeRedisKey, PropNameRedisKey, FieldNameRedisKey);
        var keyTypes = new Type[keys.Count];
        var hasKeySetter = false;
        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            keyTypes[i] = key.PropertyType;
            if (key.SetMethod != null) hasKeySetter = true;
        }

        Expression eBody;
        if (hasKeySetter)
        {
            var methodRebuild = GetMethod(keys.Count, TypeKeyRebuilder, nameof(IKeyRebuilder.RebuildKey)).MakeGenericMethod(keyTypes);
            var eRedisKeyBits = GetPropOrField(eEntity, entityType, TypeRedisKeyBits, PropNameRedisKeyBits, FieldNameRedisKeyBits);
            var eArgs = new Expression[2 + keys.Count];
            eArgs[0] = eRedisKey;
            eArgs[1] = eRedisKeyBits;
            for (int i = 0; i < keys.Count; i++)
            {
                eArgs[i + 2] = Expression.Property(eEntity, keys[i]);
            }

            eBody = Expression.Block(
                        Expression.IfThen(
                            Expression.Or(
                                Expression.GreaterThan(eRedisKeyBits, ZeroRedisKeyBits),
                                Expression.Equal(eRedisKey, NullRedisKey)
                            ),
                            Expression.Block(
                                Expression.Assign(eRedisKey, Expression.Call(ParameterKeyRebuilder, methodRebuild, eArgs)),
                                Expression.Assign(eRedisKeyBits, ZeroRedisKeyBits)
                            )
                        ),
                        eRedisKey
                    );
        }
        else
        {
            var methodBuild = GetMethod(keys.Count, TypeKeyBuilder, nameof(IKeyBuilder.BuildKey)).MakeGenericMethod(keyTypes);
            var eArgs = new Expression[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                eArgs[i] = Expression.Property(eEntity, keys[i]);
            }
            eBody = Expression.Coalesce(
                eRedisKey,
                Expression.Assign(eRedisKey, Expression.Call(ParameterKeyRebuilder, methodBuild, eArgs))
            );
        }

        return Expression.Lambda<KeyReader<TEntity>>(eBody, eEntity, ParameterKeyRebuilder).Compile();
    }

    private static Expression GetPropOrField(ParameterExpression eEntity, Type entityType,
        Type memberType, string propName, string fieldName)
    {
        if (entityType.IsInterface)
        {
            var property = entityType.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);

            if (property == null || property.PropertyType != memberType || property.GetMethod == null || property.SetMethod == null)
                throw new InvalidOperationException($"Entity type '{entityType.FullName}' does not contain public property '{propName}' with type '{memberType.FullName}' and get/set methods");

            return Expression.Property(eEntity, property);
        }
        else
        {
            var field = entityType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null || field.FieldType != memberType || field.IsInitOnly)
                throw new InvalidOperationException($"Entity type '{entityType.FullName}' does not contain non-public and non-readonly field '{fieldName}' with type '{memberType.FullName}'");

            return Expression.Field(eEntity, field);
        }
    }

    private static MethodInfo GetMethod(int count, Type type, string name)
    {
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
        for (int i = 0; i < methods.Length; i++)
        {
            var method = methods[i];
            if (method.Name == name && method.ContainsGenericParameters)
            {
                var args = method.GetGenericArguments();
                if (args.Length == count) return method;
            }
        }

        throw new InvalidOperationException($"Method '{name}' with {count} arguments not found");
    }
}