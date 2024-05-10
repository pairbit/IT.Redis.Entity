namespace IT.Redis.Entity;

public delegate RedisValue RedisValueReader<T>(T entity, IRedisValueSerializer serializer);