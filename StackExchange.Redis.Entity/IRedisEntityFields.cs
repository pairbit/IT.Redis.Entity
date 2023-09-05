﻿namespace StackExchange.Redis.Entity;

public interface IRedisEntityFields : IReadOnlyDictionary<string, RedisValue>
{
    RedisValue[] All { get; }
}