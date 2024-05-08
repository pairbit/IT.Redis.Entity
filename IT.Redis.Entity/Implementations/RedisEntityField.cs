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

    public PropertyInfo Property => _propertyInfo;

    public RedisValue RedisField => _redisField;

    public bool CanRead => _reader != null;

    public bool CanWrite => _writer != null;

    internal RedisEntityField(PropertyInfo propertyInfo, RedisValue redisField,
        IRedisValueFormatter formatter)
    {
        _propertyInfo = propertyInfo;
        _redisField = redisField;
        _formatterGeneric = RedisValueFormatterProxy.GetFormatterGeneric(propertyInfo.PropertyType, formatter);

        if (propertyInfo.SetMethod != null)
        {
            _writer = Compiler.GetWriter<TEntity>(propertyInfo);
            _deserializer = new RedisValueDeserializerProxy(formatter);
        }

        if (propertyInfo.GetMethod != null)
        {
            _reader = Compiler.GetReader<TEntity>(propertyInfo);
            _serializer = formatter;
        }
    }

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
}