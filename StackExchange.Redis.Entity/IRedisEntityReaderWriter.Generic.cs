namespace StackExchange.Redis.Entity;

public interface IRedisEntityReaderWriter<T> : IRedisEntityReader<T>, IRedisEntityWriter<T>
{
}