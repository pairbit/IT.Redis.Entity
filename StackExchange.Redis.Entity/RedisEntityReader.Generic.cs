namespace StackExchange.Redis.Entity;

public class RedisEntityReader<T> : IRedisEntityReader<T>
{
    public IRedisEntityFields Fields => throw new NotImplementedException();

    private readonly Func<T, RedisValue>[] _readers;

    public RedisValue Read(T entity, in RedisValue field) => _readers[(int)field](entity);
}