using StackExchange.Redis.Entity.Internal;

namespace StackExchange.Redis.Entity;

public class RedisEntityReaderWriter<T> : IRedisEntityReaderWriter<T>
{
    class WriterInfo
    {
        public RedisValueWriter<T> Writer { get; set; } = null!;

        public RedisValueDeserializerProxy Deserializer { get; set; } = null!;
    }

    class ReaderInfo
    {
        public RedisValueReader<T> Reader { get; set; } = null!;

        public IRedisValueSerializer Serializer { get; set; } = null!;
    }

    private readonly Dictionary<RedisValue, ReaderInfo> _readerInfos;
    private readonly Dictionary<RedisValue, WriterInfo> _writerInfos;
    private readonly IRedisEntityFields _readerFields;
    private readonly IRedisEntityFields _writerFields;

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
        var set = new HashSet<RedisValue>(properties.Length);

        foreach (var property in properties)
        {
            var field = configuration.GetField(property);

            if (!field.IsNull)
            {
                var name = property.Name;

                if (!set.Add(field)) throw new InvalidOperationException($"Propery '{name}' has duplicate '{field}'");

                var formatter = configuration.GetFormatter(property);
                
                if (property.SetMethod != null)
                {
                    writerInfos.Add(field, new WriterInfo
                    {
                        Writer = Compiler.CompileWriter<T>(property),
                        Deserializer = new RedisValueDeserializerProxy(formatter)
                    });
                    writerFields.Add(name, field);
                }

                if (property.GetMethod != null)
                {
                    readerInfos.Add(field, new ReaderInfo
                    {
                        Reader = Compiler.CompileReader<T>(property),
                        Serializer = formatter
                    });
                    readerFields.Add(name, field);
                }
            }
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
}