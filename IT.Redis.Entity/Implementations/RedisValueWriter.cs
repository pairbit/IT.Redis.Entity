namespace IT.Redis.Entity;

public delegate void RedisValueWriter<TEntity>(TEntity entity, RedisValue value, RedisValueDeserializerProxy deserializer);