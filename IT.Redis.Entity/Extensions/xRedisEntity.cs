﻿namespace IT.Redis.Entity.Extensions;

public static class xRedisEntity
{
    public static RedisValue[] GetCountAndFieldsAndValues<TEntity>(this RedisEntity<TEntity>[] redisEntities, TEntity[] entities, int offset = 0)
    {
        if (redisEntities.Length != entities.Length) throw new ArgumentOutOfRangeException(nameof(entities));
        //TODO: calc length??
        var values = new List<RedisValue>();
        for (int i = 0; i < offset; i++)
        {
            values.Add(default);
        }
        for (int i = 0; i < redisEntities.Length; i++)
        {
            var entity = entities[i];
            var fields = redisEntities[i].ReadFields.Array;
            values.Add(fields.Length << 1);
            for (int f = 0; f < fields.Length; f++)
            {
                var field = fields[f];
                values.Add(field.RedisValue);
                values.Add(field.Read(entity));
            }
        }
        return [.. values];
    }

    public static RedisValue[] GetCountAndFields<TEntity>(this RedisEntity<TEntity>[] redisEntities, int offset = 0)
    {
        //TODO: calc length?
        var values = new List<RedisValue>();
        for (int i = 0; i < offset; i++)
        {
            values.Add(default);
        }
        for (int i = 0; i < redisEntities.Length; i++)
        {
            var fields = redisEntities[i].WriteFields.RedisValues;
            values.Add(fields.Length);
            values.AddRange(fields);
        }
        return [.. values];
    }

    public static TEntity?[] GetEntities<TEntity, IEntity>(this RedisEntity<IEntity>[] redisEntities, RedisValue[] values, Func<TEntity> newEntity, int offset = 0) where TEntity : IEntity
    {
        if (redisEntities.Length == 0) return [];
        var entities = new TEntity?[redisEntities.Length];

        for (int i = 0; i < entities.Length; i++)
        {
            var fields = redisEntities[i].WriteFields.Array;
            entities[i] = fields.GetEntity(values, newEntity, offset);
            offset += fields.Length;
        }

        return entities;
    }

    public static TEntity?[] GetEntities<TEntity>(this RedisEntity<TEntity>[] redisEntities, RedisValue[] values, Func<TEntity> newEntity, int offset = 0)
        => redisEntities.GetEntities<TEntity, TEntity>(values, newEntity, offset);

    public static TEntity?[] GetEntities<TEntity, IEntity>(this RedisEntity<IEntity>[] redisEntities, RedisValue[] values, int offset = 0) where TEntity : IEntity, new()
        => redisEntities.GetEntities(values, static () => new TEntity(), offset);

    public static TEntity?[] GetEntities<TEntity>(this RedisEntity<TEntity>[] redisEntities, RedisValue[] values, int offset = 0) where TEntity : new()
        => redisEntities.GetEntities<TEntity, TEntity>(values, offset);
}