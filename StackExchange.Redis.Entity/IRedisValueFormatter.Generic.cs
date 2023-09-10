namespace StackExchange.Redis.Entity;

public interface IRedisValueFormatter<T> : IRedisValueSerializer<T>, IRedisValueDeserializer<T>
{
}