using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntityField<TEntity>
{
    private readonly PropertyInfo _propertyInfo;
    private readonly RedisValue _field;

    private readonly object _formatterGeneric;

    private readonly RedisValueWriter<TEntity>? _writer;
    private readonly RedisValueDeserializerProxy? _deserializer;

    private readonly RedisValueReader<TEntity>? _reader;
    private readonly IRedisValueSerializer? _serializer;

    public PropertyInfo Property => _propertyInfo;

    public RedisValue Field => _field;

    public bool CanRead => _reader != null;

    public bool CanWrite => _writer != null;

    internal RedisEntityField(PropertyInfo property, RedisValue field,
        IRedisValueFormatter formatter)
    {
        _propertyInfo = property;
        _field = field;
        _formatterGeneric = RedisValueFormatterProxy.GetFormatterGeneric(property.PropertyType, formatter);

        if (property.SetMethod != null)
        {
            _writer = Compiler.GetWriter<TEntity>(property);
            _deserializer = new RedisValueDeserializerProxy(formatter);
        }

        if (property.GetMethod != null)
        {
            _reader = Compiler.GetReader<TEntity>(property);
            _serializer = formatter;
        }
    }

    public RedisValue Read(TEntity entity)
    {
        return (_reader ?? throw new InvalidOperationException())(entity, _serializer!);
    }

    public bool Write(TEntity entity, in RedisValue value)
    {
        if (value.IsNull) return false;

        (_writer ?? throw new InvalidOperationException())(entity, value, _deserializer!);

        return true;
    }

    public IRedisValueFormatter<TField> GetFormatter<TField>()
        => (IRedisValueFormatter<TField>)_formatterGeneric;
}