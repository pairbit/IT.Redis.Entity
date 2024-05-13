using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity.Extensions;

public static class xIRedisEntityConfiguration
{
    public static RedisEntity<TEntity> New<TEntity>(this IRedisEntityConfiguration configuration)
    {
        var type = typeof(TEntity);
        var properties = configuration.GetProperties(type);
        var fields = configuration.GetFields<TEntity>(properties);
        var keys = configuration.GetKeys(properties);
        var keyBuilder = configuration.GetKeyBuilder(type, keys);
        var keyReader = configuration.GetKeyReader<TEntity>(keys);
        return new RedisEntity<TEntity>(fields, keyBuilder, keyReader);
    }

    internal static PropertyInfo[] GetProperties(this IRedisEntityConfiguration configuration, Type type)
        => type.GetProperties(configuration.GetBindingFlags(type))
               .Where(x => !configuration.IsIgnore(x)).ToArray();

    private static RedisEntityFields<TEntity> GetFields<TEntity>(this IRedisEntityConfiguration configuration, PropertyInfo[] properties)
    {
        if (properties.Length == 0) throw new InvalidOperationException();
        var dic = new Dictionary<string, RedisEntityField<TEntity>>(properties.Length);

#if NETSTANDARD2_0 || NET461
        var set = new HashSet<RedisValue>();
#else
        var set = new HashSet<RedisValue>(properties.Length);
#endif

        foreach (var property in properties)
        {
            if (!configuration.IsKey(property) &&
                configuration.TryGetField(property, out var redisField))
            {
                var propertyName = property.Name;

                if (!set.Add(redisField)) throw new InvalidOperationException($"Property '{propertyName}' has duplicate '{redisField}'");

                dic.Add(propertyName, configuration.GetField<TEntity>(property, redisField));
            }
        }

        return new RedisEntityFields<TEntity>(dic);
    }

    private static RedisEntityField<TEntity> GetField<TEntity>(this IRedisEntityConfiguration configuration,
        PropertyInfo property, RedisValue redisField)
    {
        var writer = configuration.GetWriter<TEntity>(property) ?? Compiler.GetWriter<TEntity>(property);
        var reader = configuration.GetReader<TEntity>(property) ?? Compiler.GetReader<TEntity>(property);

        if (writer == null && reader == null)
            throw new InvalidOperationException($"Writer/Reader not found for property '{property.Name}'");

        var formatter = configuration.GetFormatter(property);

        return new RedisEntityField<TEntity>(property, redisField, writer, reader, formatter);
    }

    private static PropertyInfo[] GetKeys(this IRedisEntityConfiguration configuration, PropertyInfo[] properties)
        => properties.Where(configuration.IsKey).ToArray();

    private static IKeyRebuilder GetKeyBuilder(this IRedisEntityConfiguration configuration, Type entityType, PropertyInfo[] keys)
    {
        //TODO: keys.length == 0 ??
        var keyBuilder = new EntityKeyRebuilder(entityType, configuration.GetKeyPrefix(entityType));
        foreach (var key in keys)
        {
            keyBuilder.AddKeyInfo(key, configuration.GetUtf8Formatter(key));
        }
        return keyBuilder;
    }

    private static KeyReader<TEntity>? GetKeyReader<TEntity>(this IRedisEntityConfiguration configuration, PropertyInfo[] keys)
    {
        var keyReader = configuration.GetKeyReader<TEntity>();
        if (keyReader != null) return keyReader;

        return Compiler.GetKeyReader<TEntity>(keys);
    }
}