using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntityFactory : IRedisEntityFactory
{
    private readonly IRedisEntityConfiguration _configuration;

    public RedisEntityFactory(IRedisEntityConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public RedisEntity<TEntity> New<TEntity>() => New<TEntity>(_configuration);

    public static RedisEntity<TEntity> New<TEntity>(IRedisEntityConfiguration configuration)
    {
        var properties = RedisEntity<TEntity>.Properties;
        var dic = new Dictionary<string, RedisEntityField<TEntity>>(properties.Length);

#if NETSTANDARD2_0 || NET461
        var set = new HashSet<RedisValue>();
#else
        var set = new HashSet<RedisValue>(properties.Length);
#endif
        var keys = new List<PropertyInfo>(EntityKeyRebuilder.MaxKeys);
        var keyBuilder = new EntityKeyRebuilder(typeof(TEntity), configuration.GetKeyPrefix(typeof(TEntity)));

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

                dic.Add(name, field);
            }
        }

        var readerKey = Compiler.GetKeyReader<TEntity>(keys);
        var allFields = new RedisEntityFields<TEntity>(dic);

        return new RedisEntity<TEntity>(allFields, keyBuilder, readerKey);
    }
}