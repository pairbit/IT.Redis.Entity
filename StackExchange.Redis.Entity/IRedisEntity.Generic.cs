namespace StackExchange.Redis.Entity;

public interface IRedisEntity<T> : IRedisEntityReader<T>, IRedisEntityWriter<T>
{
}