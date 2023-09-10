namespace StackExchange.Redis.Entity;

public interface IRedisEntityFactory
{
    IRedisEntityReaderWriter<T> NewReaderWriter<T>();
}