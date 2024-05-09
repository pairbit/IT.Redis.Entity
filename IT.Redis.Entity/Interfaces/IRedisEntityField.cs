using System.Reflection;

namespace IT.Redis.Entity;

public interface IRedisEntityField<TEntity>
{
    public PropertyInfo Property { get; }

    public RedisValue ForRedis { get; }

    public bool CanRead { get; }

    public bool CanWrite { get; }

    RedisValue Read(TEntity entity);

    bool Write(TEntity entity, in RedisValue value);

    IRedisValueFormatter<TField> GetFormatter<TField>();
}