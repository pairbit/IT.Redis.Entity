namespace IT.Redis.Entity;

public class RedisEntityFactory : IRedisEntityFactory
{
    private readonly IRedisEntityConfiguration _configuration;

    public RedisEntityFactory(IRedisEntityConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IRedisEntity<TEntity> New<TEntity>()
        => new RedisEntityImpl<TEntity>(_configuration);
}