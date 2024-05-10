using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntityImpl<TEntity> : IRedisEntity<TEntity>
{
    private readonly RedisEntityFields<TEntity> _fields;
    private readonly Func<TEntity, IKeyBuilder, byte[]>? _readerKey;
    private readonly EntityKeyBuilder _keyBuilder;

    public IKeyBuilder KeyBuilder => _keyBuilder;

    public RedisEntityFields<TEntity> Fields => _fields;

    public RedisEntityImpl(IRedisEntityConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var properties = RedisEntity<TEntity>.Properties;
        var fields = new Dictionary<string, RedisEntityField<TEntity>>(properties.Length);

#if NETSTANDARD2_0 || NET461
        var set = new HashSet<RedisValue>();
#else
        var set = new HashSet<RedisValue>(properties.Length);
#endif
        var keys = new List<PropertyInfo>(EntityKeyBuilder.MaxKeys);
        var keyBuilder = new EntityKeyBuilder(typeof(TEntity), configuration.GetKeyPrefix(typeof(TEntity)));

        foreach (var property in properties)
        {
            var redisField = configuration.GetField(property, out var hasKey);

            if (hasKey)
            {
                keys.Add(property);
                keyBuilder.AddKeyInfo(property, configuration.GetUtf8Formatter(property));
            }
            else if (!redisField.IsNull)
            {
                var name = property.Name;

                if (!set.Add(redisField)) throw new InvalidOperationException($"Property '{name}' has duplicate '{redisField}'");

                var formatter = configuration.GetFormatter(property);
                var writer = configuration.GetWriter<TEntity>(property);
                var reader = configuration.GetReader<TEntity>(property);

                if (writer == null && reader == null)
                    throw new InvalidOperationException($"Writer/Reader not found for property '{name}'");

                var field = new RedisEntityField<TEntity>(property, redisField,
                    writer, reader, formatter);

                fields.Add(name, field);
            }
        }

        if (keys.Count > 0) _readerKey = Compiler.GetReaderKey<TEntity>(keys);
        _keyBuilder = keyBuilder;
        _fields = new RedisEntityFields<TEntity>(fields);
    }

    public RedisKey ReadKey(TEntity entity)
    {
        return (_readerKey ?? throw new InvalidOperationException("Key not found"))
            (entity, _keyBuilder);
    }
}