namespace IT.Redis.Entity.Internal;

public delegate void RedisValueWriter<T>(T entity, RedisValue value, RedisValueDeserializerProxy deserializer);