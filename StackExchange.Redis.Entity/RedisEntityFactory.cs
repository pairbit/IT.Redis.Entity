namespace StackExchange.Redis.Entity;

public class RedisEntityFactory : IRedisEntityFactory
{
    private readonly IRedisEntityConfiguration _configuration;

    public RedisEntityFactory(IRedisEntityConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IRedisEntityReaderWriter<T> NewReaderWriter<T>() => new RedisEntityReaderWriter<T>(_configuration);
}