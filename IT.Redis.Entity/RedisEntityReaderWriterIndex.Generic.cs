using IT.Redis.Entity.Internal;

namespace IT.Redis.Entity;

public class RedisEntityReaderWriterIndex<T> : IRedisEntityReaderWriter<T>
{
    private readonly RedisEntityReaderWriter<T>.ReaderInfo[] _readerInfos;
    private readonly RedisEntityReaderWriter<T>.WriterInfo[] _writerInfos;
    private readonly IRedisEntityFields _readerFields;
    private readonly IRedisEntityFields _writerFields;

    IRedisEntityFields IRedisEntityReader<T>.Fields => _readerFields;

    IRedisEntityFields IRedisEntityWriter<T>.Fields => _writerFields;

    public RedisEntityReaderWriterIndex(IRedisEntityConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var properties = RedisEntity<T>.Properties;
        var readerFields = new Dictionary<string, RedisValue>(properties.Length);
        var writerFields = new Dictionary<string, RedisValue>(properties.Length);
        var readerInfos = new Dictionary<int, RedisEntityReaderWriter<T>.ReaderInfo>(properties.Length);
        var writerInfos = new Dictionary<int, RedisEntityReaderWriter<T>.WriterInfo>(properties.Length);

#if NETSTANDARD2_0 || NET461
        var set = new HashSet<int>();
#else
        var set = new HashSet<int>(properties.Length);
#endif
        foreach (var property in properties)
        {
            var field = configuration.GetField(property);

            if (!field.IsNull)
            {
                if (!field.IsInteger) throw new NotSupportedException("Non-integer fields are not supported");

                var index = (int)field;
                var name = property.Name;

                if (!set.Add(index)) throw new InvalidOperationException($"Propery '{name}' has duplicate '{index}'");

                var formatter = configuration.GetFormatter(property);
                var propertyType = property.PropertyType;

                if (property.SetMethod != null)
                {
                    writerInfos.Add(index, new RedisEntityReaderWriter<T>.WriterInfo
                    {
                        Writer = Compiler.GetWriter<T>(property),
                        Deserializer = new RedisValueDeserializerProxy(formatter),
                        DeserializerGeneric = Activator.CreateInstance(typeof(RedisValueDeserializerProxy<>).MakeGenericType(propertyType), formatter)!
                    });
                    writerFields.Add(name, field);
                }

                if (property.GetMethod != null)
                {
                    readerInfos.Add(index, new RedisEntityReaderWriter<T>.ReaderInfo
                    {
                        Reader = Compiler.GetReader<T>(property),
                        Serializer = formatter,
                        SerializerGeneric = Activator.CreateInstance(typeof(RedisValueSerializerProxy<>).MakeGenericType(propertyType), formatter)!
                    });
                    readerFields.Add(name, field);
                }
            }
        }

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
            throw new ArgumentException($"Field '{field}' not integer", nameof(field), ex);
        }

        if (index < 0 || index > _readerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var readerInfo = _readerInfos[index];

        if (readerInfo == null) throw new ArgumentOutOfRangeException(nameof(field));

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
            throw new ArgumentException($"Field '{field}' not integer", nameof(field), ex);
        }

        if (index < 0 || index > _writerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var writerInfo = _writerInfos[index];

        if (writerInfo == null) throw new ArgumentOutOfRangeException(nameof(field));

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
            throw new ArgumentException(nameof(field), ex);
        }

        if (index < 0 || index > _readerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var readerInfo = _readerInfos[index];

        if (readerInfo == null) throw new ArgumentOutOfRangeException(nameof(field));

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
            throw new ArgumentException(nameof(field), ex);
        }

        if (index < 0 || index > _writerInfos.Length) throw new ArgumentOutOfRangeException(nameof(field));

        var writerInfo = _writerInfos[index];

        if (writerInfo == null) throw new ArgumentOutOfRangeException(nameof(field));

        return (IRedisValueDeserializer<TField>)writerInfo.DeserializerGeneric;
    }
}