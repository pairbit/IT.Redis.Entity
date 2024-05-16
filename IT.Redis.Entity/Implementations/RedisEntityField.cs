using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntityField<TEntity>
{
    private readonly PropertyInfo _propertyInfo;
    private readonly RedisValue _redisField;

    private readonly object _formatterGeneric;

    private readonly RedisValueWriter<TEntity>? _writer;
    private readonly RedisValueDeserializerProxy? _deserializer;

    private readonly RedisValueReader<TEntity>? _reader;
    private readonly IRedisValueSerializer? _serializer;

    private readonly KeyReader<TEntity>? _keyReader;
    private readonly IKeyRebuilder _keyBuilder;

    public PropertyInfo Property => _propertyInfo;

    public RedisValue ForRedis => _redisField;

    public IKeyRebuilder KeyBuilder => _keyBuilder;

    public bool CanReadKey => _keyReader != null;

    public bool CanRead => _reader != null;

    public bool CanWrite => _writer != null;

    internal RedisEntityField(PropertyInfo propertyInfo, RedisValue redisField,
        RedisValueWriter<TEntity>? writer,
        RedisValueReader<TEntity>? reader,
        IRedisValueFormatter formatter,
        IKeyRebuilder keyBuilder, KeyReader<TEntity>? keyReader)
    {
        _propertyInfo = propertyInfo;
        _redisField = redisField;
        _formatterGeneric = RedisValueFormatterProxy.GetFormatterGeneric(propertyInfo.PropertyType, formatter);
        _keyBuilder = keyBuilder;
        _keyReader = keyReader;

        if (writer != null)
        {
            _writer = writer;
            _deserializer = new RedisValueDeserializerProxy(formatter);
        }

        if (reader != null)
        {
            _reader = reader;
            _serializer = formatter;
        }
    }

    public RedisKey ReadKey(TEntity entity) => (_keyReader ?? throw new InvalidOperationException("Key not found"))
        (entity, _keyBuilder);

    public RedisValue Read(TEntity entity)
    {
        return (_reader ?? throw new InvalidOperationException("WriteOnly"))(entity, _serializer!);
    }

    public bool Write(TEntity entity, in RedisValue value)
    {
        if (value.IsNull) return false;

        (_writer ?? throw new InvalidOperationException("ReadOnly"))(entity, value, _deserializer!);

        return true;
    }

    public IRedisValueFormatter<TField> GetFormatter<TField>()
        => (IRedisValueFormatter<TField>)_formatterGeneric;

    public override string ToString()
    {
        var canRead = _reader != null;
        var canWrite = _writer != null;

        var prefix = string.Empty;
        if (canRead && !canWrite) prefix = "[R] ";
        if (!canRead && canWrite) prefix = "[W] ";

        var prop = _propertyInfo.Name;
        var redis = (string)_redisField!;

        return prop.Equals(redis) ? $"{prefix}{prop}" : $"{prefix}{prop} -> {redis}";
    }
}