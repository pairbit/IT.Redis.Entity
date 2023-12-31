﻿namespace IT.Redis.Entity;

public static class RedisEntity
{
    private static IRedisEntityFactory _factory = new RedisEntityFactory(new Configurations.AnnotationConfiguration());

    public static IRedisEntityFactory Factory
    {
        get { return _factory; }
        set { _factory = value ?? throw new ArgumentNullException(nameof(value)); }
    }
}