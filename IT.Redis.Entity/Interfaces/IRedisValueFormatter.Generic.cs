namespace IT.Redis.Entity;

public interface IRedisValueFormatter<T> : IRedisValueSerializer<T>, IRedisValueDeserializer<T>
{
}