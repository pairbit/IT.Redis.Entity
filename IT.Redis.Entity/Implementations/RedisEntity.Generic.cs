using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntity<T>
{
    internal static readonly PropertyInfo[] Properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private static Lazy<IRedisEntityReaderWriter<T>> _readerWriter = new(() => RedisEntity.Factory.NewReaderWriter<T>());
    private static Lazy<IRedisEntityReader<T>> _reader = new(() => _readerWriter.Value);
    private static Lazy<IRedisEntityWriter<T>> _writer = new(() => _readerWriter.Value);

    public static Func<IRedisEntityReaderWriter<T>> ReaderWriterFactory
    {
        set
        {
            _readerWriter = new(value ?? throw new ArgumentNullException(nameof(value)));
            _reader = new(() => _readerWriter.Value);
            _writer = new(() => _readerWriter.Value);
        }
    }

    public static Func<IRedisEntityReader<T>> ReaderFactory
    {
        set => _reader = new(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public static Func<IRedisEntityWriter<T>> WriterFactory
    {
        set => _writer = new(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public static IRedisEntityReaderWriter<T> ReaderWriter => _readerWriter.Value;

    public static IRedisEntityReader<T> Reader => _reader.Value;

    public static IRedisEntityWriter<T> Writer => _writer.Value;
}