using StackExchange.Redis.Entity.Formatters;
using StackExchange.Redis.Entity.Internal;

namespace StackExchange.Redis.Entity;

public static class RedisValueFormatterRegistry
{
    static class Cache<T>
    {
        public static IRedisValueFormatter<T>? _formatter;
    }

    private static IRedisValueFormatter _default = new RedisValueFormatterNotRegistered();

    public static IRedisValueFormatter Default => _default;

    static RedisValueFormatterRegistry()
    {
        Register(StringFormatter.Default);
        Register(EnumFormatter.Default);
        Register(ByteArrayFormatter.Default);

        Register(RedisValueFormatter.Default);

        Register(SByteFormatter.Default);
        Register(ByteFormatter.Default);
        Register(Int16Formatter.Default);
        Register(UInt16Formatter.Default);
        Register(Int32Formatter.Default);
        Register(UInt32Formatter.Default);
        Register(Int64Formatter.Default);
        Register(UInt64Formatter.Default);
        Register(SingleFormatter.Default);
        Register(DoubleFormatter.Default);
        Register(DecimalFormatter.Default);
        Register(BigIntegerFormatter.Default);

        Register(CharFormatter.Default);
        Register(BooleanFormatter.Default);
        Register(IntPtrFormatter.Default);
        Register(UIntPtrFormatter.Default);

        Register(DateTimeFormatter.Default);
        Register(DateTimeOffsetFormatter.Default);
        Register(TimeSpanFormatter.Default);
        Register(GuidFormatter.Default);

        Register(ReadOnlyMemoryByteFormatter.Default);
        Register(DateOnlyFormatter.Default);
        Register(TimeOnlyFormatter.Default);
    }

    public static void Register<T>(IRedisValueFormatter<T> formatter)
    {
        Cache<T>._formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public static void Register<T>(IStructFormatter<T> formatter) where T : struct
    {
        if (formatter == null) throw new ArgumentNullException(nameof(formatter));
        Cache<T>._formatter = formatter;
        Cache<T?>._formatter = formatter;
    }

    public static void Register(IRedisValueFormatter formatter)
    {
        _default = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public static IRedisValueFormatter<T>? GetFormatter<T>() => Cache<T>._formatter;
}