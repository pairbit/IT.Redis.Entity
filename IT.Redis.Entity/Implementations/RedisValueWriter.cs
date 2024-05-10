namespace IT.Redis.Entity;

public delegate void RedisValueWriter<T>(T entity, RedisValue value, RedisValueDeserializerProxy deserializer);