namespace IT.Redis.Entity.Internal;

public delegate RedisValue RedisValueReader<T>(T entity, IRedisValueSerializer serializer);