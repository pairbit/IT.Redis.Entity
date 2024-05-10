using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity;

public interface IRedisEntityConfiguration
{
    string? GetKeyPrefix(Type type);

    RedisValue GetField(PropertyInfo property, out bool hasKey);

    IRedisValueFormatter GetFormatter(PropertyInfo property);
    
    RedisValueWriter<TEntity>? GetWriter<TEntity>(PropertyInfo property);

    RedisValueReader<TEntity>? GetReader<TEntity>(PropertyInfo property);

    object GetUtf8Formatter(PropertyInfo property);
}