﻿namespace IT.Redis.Entity.Formatters;

public abstract class NullableFormatter<T> : IStructFormatter<T> where T : struct
{
    public void Deserialize(in RedisValue redisValue, ref T? value)
    {
        if (redisValue.IsNullOrEmpty)
        {
            value = null;
        }
        else
        {
            T valueNotNull = default;
            DeserializeNotNull(in redisValue, ref valueNotNull);
            value = valueNotNull;
        }
    }

    public void Deserialize(in RedisValue redisValue, ref T value)
    {
        if (redisValue.IsNullOrEmpty)
        {
            value = default;
        }
        else
        {
            DeserializeNotNull(in redisValue, ref value);
        }
    }

    public abstract void DeserializeNotNull(in RedisValue redisValue, ref T value);

    public RedisValue Serialize(in T? value) => value == null ? RedisValue.EmptyString : Serialize(value.Value);

    public abstract RedisValue Serialize(in T value);
}