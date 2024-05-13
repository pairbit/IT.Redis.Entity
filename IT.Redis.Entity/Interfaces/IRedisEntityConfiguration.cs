using System.Reflection;

namespace IT.Redis.Entity;

public interface IRedisEntityConfiguration
{
    string? GetKeyPrefix(Type type);

    BindingFlags GetBindingFlags(Type type);

    KeyReader<TEntity>? GetKeyReader<TEntity>();

    bool IsIgnore(PropertyInfo property);

    bool IsKey(PropertyInfo property);

    bool TryGetField(PropertyInfo property, out RedisValue field);

    IRedisValueFormatter GetFormatter(PropertyInfo property);
    
    RedisValueWriter<TEntity>? GetWriter<TEntity>(PropertyInfo property);

    RedisValueReader<TEntity>? GetReader<TEntity>(PropertyInfo property);

    object GetUtf8Formatter(PropertyInfo property);
}