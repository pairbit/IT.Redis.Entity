namespace StackExchange.Redis.Entity;

public class RedisEntity<T>
{
    public static IRedisEntityFields Fields { get; }

    public static IRedisEntityWriter<T> Writer { get; set; }

    public static IRedisEntityReader<T> Reader { get; set; }
}