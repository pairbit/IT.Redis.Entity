using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntityReaderWriterIndex<T> : IRedisEntityReaderWriter<T>
{
    private readonly RedisEntityReaderWriter<T>.ReaderInfo[] _readerInfos;
    private readonly RedisEntityReaderWriter<T>.WriterInfo[] _writerInfos;
    private readonly IRedisEntityFields _fields;
    private readonly IRedisEntityFields _readerFields;
    private readonly IRedisEntityFields _writerFields;
    private readonly Func<T, EntityKeyBuilder, byte[]>? _readerKey;
    private readonly EntityKeyBuilder _keyBuilder;

    IRedisEntityFields IRedisEntityReader<T>.Fields => _readerFields;

    IRedisEntityFields IRedisEntityWriter<T>.Fields => _writerFields;

    public IRedisEntityFields Fields => _fields;

    public IKeyBuilder KeyBuilder => _keyBuilder;

    public RedisEntityReaderWriterIndex(IRedisEntityConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var properties = RedisEntity<T>.Properties;
        var fields = new Dictionary<string, RedisValue>(properties.Length);
        var readerFields = new Dictionary<string, RedisValue>(properties.Length);
        var writerFields = new Dictionary<string, RedisValue>(properties.Length);
        var readerInfos = new Dictionary<int, RedisEntityReaderWriter<T>.ReaderInfo>(properties.Length);
        var writerInfos = new Dictionary<int, RedisEntityReaderWriter<T>.WriterInfo>(properties.Length);

#if NETSTANDARD2_0 || NET461
        var set = new HashSet<int>();
#else
        var set = new HashSet<int>(properties.Length);
#endif
        var keys = new List<PropertyInfo>();
        var keyBuilder = new EntityKeyBuilder(typeof(T), configuration.GetKeyPrefix(typeof(T)));

        foreach (var property in properties)
        {
            var field = configuration.GetField(property, out var hasKey);

            if (hasKey)
            {
                keys.Add(property);
                keyBuilder.AddKeyInfo(property, configuration.GetUtf8Formatter(property));
            }
            else if (!field.IsNull)
            {
                if (!field.IsInteger) throw new NotSupportedException("Non-integer fields are not supported");

                var index = (int)field;
                var name = property.Name;

                if (!set.Add(index)) throw new InvalidOperationException($"Propery '{name}' has duplicate '{index}'");

                fields.Add(name, field);
                var formatter = configuration.GetFormatter(property);
                var formatterGeneric = Activator.CreateInstance(typeof(RedisValueFormatterProxy<>).MakeGenericType(property.PropertyType), formatter)!;

                if (property.SetMethod != null)
                {
                    writerInfos.Add(index, new RedisEntityReaderWriter<T>.WriterInfo
                    {
                        Writer = Compiler.GetWriter<T>(property),
                        Deserializer = new RedisValueDeserializerProxy(formatter),
                        DeserializerGeneric = formatterGeneric
                    });
                    writerFields.Add(name, field);
                }

                if (property.GetMethod != null)
                {
                    readerInfos.Add(index, new RedisEntityReaderWriter<T>.ReaderInfo
                    {
                        Reader = Compiler.GetReader<T>(property),
                        Serializer = formatter,
                        SerializerGeneric = formatterGeneric
                    });
                    readerFields.Add(name, field);
                }
            }
        }
        if (keys.Count > 0) _readerKey = Compiler.GetReaderKey<T>(keys);
        _keyBuilder = keyBuilder;
        _fields = new RedisEntityFields(fields);
        _readerFields = new RedisEntityFields(readerFields);
        _writerFields = new RedisEntityFields(writerFields);

        var max = set.Max() + 1;
        _readerInfos = new RedisEntityReaderWriter<T>.ReaderInfo[max];
        _writerInfos = new RedisEntityReaderWriter<T>.WriterInfo[max];

        foreach (var readerInfo in readerInfos)
        {
            _readerInfos[readerInfo.Key] = readerInfo.Value;
        }

        foreach (var writerInfo in writerInfos)
        {
            _writerInfos[writerInfo.Key] = writerInfo.Value;
        }
    }

    public RedisValue Read(T entity, in RedisValue field)
    {
        int index;
        try
        {
            index = (int)field;
        }
        catch (InvalidCastException ex)
        {
            throw Ex.FieldNotInteger(field, ex, nameof(field));
        }

        if (index < 0 || index > _readerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var readerInfo = _readerInfos[index] ?? throw new ArgumentOutOfRangeException(nameof(field));

        return readerInfo.Reader(entity, readerInfo.Serializer);
    }

    public bool Write(T entity, in RedisValue field, in RedisValue value)
    {
        if (value.IsNull) return false;

        int index;
        try
        {
            index = (int)field;
        }
        catch (InvalidCastException ex)
        {
            throw Ex.FieldNotInteger(field, ex, nameof(field));
        }

        if (index < 0 || index > _writerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var writerInfo = _writerInfos[index] ?? throw new ArgumentOutOfRangeException(nameof(field));

        writerInfo.Writer(entity, value, writerInfo.Deserializer);

        return true;
    }

    public IRedisValueSerializer<TField> GetSerializer<TField>(in RedisValue field)
    {
        int index;
        try
        {
            index = (int)field;
        }
        catch (InvalidCastException ex)
        {
            throw Ex.FieldNotInteger(field, ex, nameof(field));
        }

        if (index < 0 || index > _readerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var readerInfo = _readerInfos[index] ?? throw new ArgumentOutOfRangeException(nameof(field));

        return (IRedisValueSerializer<TField>)readerInfo.SerializerGeneric;
    }

    public IRedisValueDeserializer<TField> GetDeserializer<TField>(in RedisValue field)
    {
        int index;
        try
        {
            index = (int)field;
        }
        catch (InvalidCastException ex)
        {
            throw Ex.FieldNotInteger(field, ex, nameof(field));
        }

        if (index < 0 || index > _writerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var writerInfo = _writerInfos[index] ?? throw new ArgumentOutOfRangeException(nameof(field));

        return (IRedisValueDeserializer<TField>)writerInfo.DeserializerGeneric;
    }

    public RedisKey ReadKey(T entity)
    {
        if (_readerKey == null) throw new InvalidOperationException("Key not found");

        return _readerKey(entity, _keyBuilder);
    }
}