namespace IT.Redis.Entity;

public delegate RedisValue RedisValueReader<TEntity>(TEntity entity, IRedisValueSerializer serializer);