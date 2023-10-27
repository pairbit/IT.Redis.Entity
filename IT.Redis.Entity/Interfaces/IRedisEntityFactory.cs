namespace IT.Redis.Entity;

public interface IRedisEntityFactory
{
    IRedisEntityReaderWriter<T> NewReaderWriter<T>();
}