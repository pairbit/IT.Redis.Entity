namespace IT.Redis.Entity;

public interface IRedisEntityReader<T> : IRedisEntityOLD<T>
{
    IRedisEntityFields Fields { get; }

    RedisValue Read(T entity, in RedisValue field);

    IRedisValueSerializer<TField> GetSerializer<TField>(in RedisValue field);
}