namespace IT.Redis.Entity;

public interface IRedisEntityWriter<T> : IRedisEntity<T>
{
    IRedisEntityFields Fields { get; }

    bool Write(T entity, in RedisValue field, in RedisValue value);

    IRedisValueDeserializer<TField> GetDeserializer<TField>(in RedisValue field);
}