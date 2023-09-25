namespace IT.Redis.Entity;

public interface IRedisValueDeserializer<T>
{
    void Deserialize(in RedisValue redisValue, ref T? value);
}