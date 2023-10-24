namespace IT.Redis.Entity;

public interface IRedisEntityWriter<T>
{
    IRedisEntityFields Fields { get; }

    RedisKey ReadKey(T entity);

    bool Write(T entity, in RedisValue field, in RedisValue value);

    IRedisValueDeserializer<TField> GetDeserializer<TField>(in RedisValue field);
}