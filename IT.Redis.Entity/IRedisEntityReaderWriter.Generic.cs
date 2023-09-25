namespace IT.Redis.Entity;

public interface IRedisEntityReaderWriter<T> : IRedisEntityReader<T>, IRedisEntityWriter<T>
{
}