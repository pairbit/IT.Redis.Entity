namespace IT.Redis.Entity;

public interface IRedisEntityReader<T>
{
    IRedisEntityFields Fields { get; }

    IKeyBuilder KeyBuilder { get; }

    RedisKey ReadKey(T entity);

    RedisValue Read(T entity, in RedisValue field);

    IRedisValueSerializer<TField> GetSerializer<TField>(in RedisValue field);
}