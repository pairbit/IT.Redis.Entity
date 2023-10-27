namespace IT.Redis.Entity;

public interface IRedisValueSerializer
{
    RedisValue Serialize<T>(in T? value);
}