using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntityReaderWriter<T> : IRedisEntityReaderWriter<T>
{
    internal class WriterInfo
    {
        public RedisValueWriter<T> Writer { get; set; } = null!;

        public RedisValueDeserializerProxy Deserializer { get; set; } = null!;

        public object DeserializerGeneric { get; set; } = null!;
    }

    internal class ReaderInfo
    {
        public RedisValueReader<T> Reader { get; set; } = null!;

        public IRedisValueSerializer Serializer { get; set; } = null!;

        public object SerializerGeneric { get; set; } = null!;
    }

    private readonly Dictionary<RedisValue, ReaderInfo> _readerInfos;
    private readonly Dictionary<RedisValue, WriterInfo> _writerInfos;
    private readonly IRedisEntityFields _readerFields;
    private readonly IRedisEntityFields _writerFields;
    private readonly Func<T, KeyBuilder, byte[]>? _readerKey;
    private readonly KeyBuilder? _keyBuilder;

    IRedisEntityFields IRedisEntityReader<T>.Fields => _readerFields;

    IRedisEntityFields IRedisEntityWriter<T>.Fields => _writerFields;

    public RedisEntityReaderWriter(IRedisEntityConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var properties = RedisEntity<T>.Properties;
        var readerFields = new Dictionary<string, RedisValue>(properties.Length);
        var writerFields = new Dictionary<string, RedisValue>(properties.Length);
        var readerInfos = new Dictionary<RedisValue, ReaderInfo>(properties.Length);
        var writerInfos = new Dictionary<RedisValue, WriterInfo>(properties.Length);
#if NETSTANDARD2_0 || NET461
        var set = new HashSet<RedisValue>();
#else
        var set = new HashSet<RedisValue>(properties.Length);
#endif
        var keys = new List<PropertyInfo>();
        var keyBuilder = new KeyBuilder(configuration.GetKeyPrefix(typeof(T)));

        foreach (var property in properties)
        {
            var field = configuration.GetField(property, out var hasKey);

            if (hasKey)
            {
                keys.Add(property);
                keyBuilder.AddSerializer(configuration.GetFixFormatter(property));
            }
            else if (!field.IsNull)
            {
                var name = property.Name;

                if (!set.Add(field)) throw new InvalidOperationException($"Propery '{name}' has duplicate '{field}'");

                var formatter = configuration.GetFormatter(property);
                var propertyType = property.PropertyType;

                if (property.SetMethod != null)
                {
                    writerInfos.Add(field, new WriterInfo
                    {
                        Writer = Compiler.GetWriter<T>(property),
                        Deserializer = new RedisValueDeserializerProxy(formatter),
                        DeserializerGeneric = Activator.CreateInstance(typeof(RedisValueDeserializerProxy<>).MakeGenericType(propertyType), formatter)!
                    });
                    writerFields.Add(name, field);
                }

                if (property.GetMethod != null)
                {
                    readerInfos.Add(field, new ReaderInfo
                    {
                        Reader = Compiler.GetReader<T>(property),
                        Serializer = formatter,
                        SerializerGeneric = Activator.CreateInstance(typeof(RedisValueSerializerProxy<>).MakeGenericType(propertyType), formatter)!
                    });
                    readerFields.Add(name, field);
                }
            }
        }

        if (keys.Count > 0)
        {
            _keyBuilder = keyBuilder;
            _readerKey = Compiler.GetReaderKey<T>(keys);
        }
        _readerFields = new RedisEntityFields(readerFields);
        _writerFields = new RedisEntityFields(writerFields);
        _readerInfos = readerInfos;
        _writerInfos = writerInfos;
    }

    public RedisValue Read(T entity, in RedisValue field)
    {
        if (!_readerInfos.TryGetValue(field, out var readerInfo))
            throw new ArgumentOutOfRangeException(nameof(field));

        return readerInfo.Reader(entity, readerInfo.Serializer);
    }

    public bool Write(T entity, in RedisValue field, in RedisValue value)
    {
        if (value.IsNull) return false;

        if (!_writerInfos.TryGetValue(field, out var writerInfo))
            throw new ArgumentOutOfRangeException(nameof(field));

        writerInfo.Writer(entity, value, writerInfo.Deserializer);

        return true;
    }

    public IRedisValueSerializer<TField> GetSerializer<TField>(in RedisValue field)
    {
        if (!_readerInfos.TryGetValue(field, out var readerInfo))
            throw new ArgumentOutOfRangeException(nameof(field));

        return (IRedisValueSerializer<TField>)readerInfo.SerializerGeneric;
    }

    public IRedisValueDeserializer<TField> GetDeserializer<TField>(in RedisValue field)
    {
        if (!_writerInfos.TryGetValue(field, out var writerInfo))
            throw new ArgumentOutOfRangeException(nameof(field));

        return (IRedisValueDeserializer<TField>)writerInfo.DeserializerGeneric;
    }

    public RedisKey ReadKey(T entity)
    {
        if (_readerKey == null) throw new InvalidOperationException("Key not found");

        return _readerKey(entity, _keyBuilder);
    }
}