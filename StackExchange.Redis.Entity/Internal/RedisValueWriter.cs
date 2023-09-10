namespace StackExchange.Redis.Entity.Internal;

internal delegate void RedisValueWriter<T>(T entity, RedisValue value, RedisValueDeserializerProxy deserializer);