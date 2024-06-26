﻿using IT.Redis.Entity.Extensions;

namespace IT.Redis.Entity;

public class RedisEntity<TEntity>
{
    private static Lazy<RedisEntity<TEntity>> _default = new(RedisEntity.Config.NewEntity<TEntity>);

    public static Func<RedisEntity<TEntity>> Factory
    {
        set
        {
            _default = new(value ?? throw new ArgumentNullException(nameof(value)));
        }
    }

    public static RedisEntity<TEntity> Default => _default.Value;

    private readonly KeyReader<TEntity>? _keyReader;
    private readonly IKeyRebuilder _keyBuilder;

    public IKeyRebuilder KeyBuilder => _keyBuilder;

    public RedisEntityFields<TEntity> AllFields { get; }

    public RedisEntityFields<TEntity> ReadFields { get; }

    public RedisEntityFields<TEntity> WriteFields { get; }

    /// <summary>
    /// Read and Write
    /// </summary>
    public RedisEntityFields<TEntity> Fields { get; }

    public bool CanReadKey => _keyReader != null;

    internal RedisEntity(RedisEntityFields<TEntity> allFields, IKeyRebuilder keyBuilder,
        KeyReader<TEntity>? keyReader)
    {
        if (allFields == null) throw new ArgumentNullException(nameof(allFields));
        var countAllFields = allFields.Count;
        if (countAllFields == 0) throw new ArgumentException("Empty", nameof(allFields));
        if (keyBuilder == null) throw new ArgumentNullException(nameof(keyBuilder));

        var countReadWriteFields = 0;
        var countReadFields = 0;
        var countWriteFields = 0;

        foreach (var field in allFields.Array)
        {
            if (field.CanRead && field.CanWrite)
            {
                countReadWriteFields++;
                countReadFields++;
                countWriteFields++;
            }
            else
            {
                if (field.CanRead) countReadFields++;
                if (field.CanWrite) countWriteFields++;
            }
        }

        ReadFields = countAllFields == countReadFields ? allFields :
                     countReadFields == 0 ? RedisEntityFields<TEntity>.Empty :
                     allFields.Sub(x => x.CanRead, countReadFields);

        WriteFields = countAllFields == countWriteFields ? allFields :
                      countWriteFields == 0 ? RedisEntityFields<TEntity>.Empty :
                      allFields.Sub(x => x.CanWrite, countWriteFields);

        Fields = countAllFields == countReadWriteFields ? allFields :
                 countReadWriteFields == 0 ? RedisEntityFields<TEntity>.Empty :
                 allFields.Sub(x => x.CanRead && x.CanWrite, countReadWriteFields);

        AllFields = allFields;
        _keyBuilder = keyBuilder;
        _keyReader = keyReader;
    }

    public RedisKey ReadKey(TEntity entity) => (_keyReader ?? throw new InvalidOperationException("Key not found"))
        (entity, _keyBuilder);

    public RedisEntity<TEntity> Sub(params string[] propertyNames)
    {
        var allFields = AllFields;
        var subFields = allFields.Sub(propertyNames);
        return subFields == allFields ? this : new(subFields, _keyBuilder, _keyReader);
    }
}