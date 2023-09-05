namespace StackExchange.Redis.Entity;

public class RedisEntity<T>
{
    public static IRedisEntityWriter<T> Writer { get; set; }

    public static IRedisEntityReader<T> Reader { get; set; }

    public static RedisValue GetField(string propName)
    {
        throw new NotImplementedException();
    }
}