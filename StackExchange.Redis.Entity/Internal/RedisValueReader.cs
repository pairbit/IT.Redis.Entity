namespace StackExchange.Redis.Entity.Internal;

internal delegate RedisValue RedisValueReader<T>(T entity, IRedisValueSerializer serializer);