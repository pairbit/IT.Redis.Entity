namespace IT.Redis.Entity;

public static class RedisEntity
{
    private static IRedisEntityConfiguration _config 
        = new Configurations.AnnotationConfiguration();

    public static IRedisEntityConfiguration Config
    {
        get { return _config; }
        set { _config = value ?? throw new ArgumentNullException(nameof(value)); }
    }
}