namespace StackExchange.Redis.Entity;

public delegate RedisValue RedisValueReader<T>(T entity);
public delegate void RedisValueWriter<T>(T entity, RedisValue value);

public class RedisEntity<T> : IRedisEntity<T>
{
    #region static

    private static IRedisEntity<T> _default = new RedisEntity<T>();
    private static Lazy<IRedisEntityReader<T>> _reader = new(_default);
    private static Lazy<IRedisEntityWriter<T>> _writer = new(_default);

    //public static IRedisEntityFields Fields { get; }

    public static Func<IRedisEntityReader<T>> ReaderFactory
    {
        set => _reader = new(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public static Func<IRedisEntityWriter<T>> WriterFactory
    {
        set => _writer = new(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public static IRedisEntity<T> Default => _default;

    public static IRedisEntityReader<T> Reader => _reader.Value;

    public static IRedisEntityWriter<T> Writer => _writer.Value;

    #endregion static

    public IRedisEntityFields Fields => throw new NotImplementedException();

    private readonly RedisValueReader<T>[] _readers;
    private readonly RedisValueWriter<T>[] _writers;

    public RedisEntity()
    {

    }

    public RedisValue Read(T entity, in RedisValue field)
    {
        var i = (int)field;
        var readers = _readers;

        if (i < 0 || i > readers.Length) throw new ArgumentOutOfRangeException(nameof(field));

        return readers[i](entity);
    }

    public bool Write(T entity, in RedisValue field, in RedisValue value)
    {
        if (value.IsNull) return false;

        var i = (int)field;
        var writers = _writers;

        if (i < 0 || i > writers.Length) throw new ArgumentOutOfRangeException(nameof(field));

        writers[i](entity, value);

        return true;
    }
}