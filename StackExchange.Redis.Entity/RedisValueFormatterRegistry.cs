using StackExchange.Redis.Entity.Formatters;
using StackExchange.Redis.Entity.Internal;
using System.Collections.ObjectModel;

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

        Register(VersionFormatter.Default);
        Register(EnumFormatter.Default);
        Register(UriFormatter.Default);

        Register(StringFormatter.Default);
        Register(ByteArrayFormatter.Default);
        Register(BitArrayFormatter.Default);

        RegisterUnmanagedEnumerable<Int32>();
        RegisterUnmanagedEnumerable<Guid>();
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

    public static void RegisterUnmanagedList<TList, T>(NewList<TList, T> factory)
        where TList : IList<T>
        where T : unmanaged
    {
        Register(new UnmanagedListFormatter<TList, T>(factory));
    }

    public static void RegisterUnmanagedEnumerable<T>() where T : unmanaged
    {
        var enumerableFormatter = new UnmanagedEnumerableFormatter<T>();
        
        Cache<T[]>._formatter = enumerableFormatter;
        Cache<T?[]>._formatter = enumerableFormatter;
        Cache<IEnumerable<T>>._formatter = enumerableFormatter;
        Cache<IReadOnlyCollection<T>>._formatter = enumerableFormatter;
        Cache<IReadOnlyList<T>>._formatter = enumerableFormatter;
        Cache<IReadOnlyList<T?>>._formatter = enumerableFormatter;
        Cache<IReadOnlySet<T>>._formatter = enumerableFormatter;
        Cache<ICollection<T>>._formatter = enumerableFormatter;
        Cache<IList<T>>._formatter = enumerableFormatter;
        Cache<IList<T?>>._formatter = enumerableFormatter;
        Cache<ISet<T>>._formatter = enumerableFormatter;
        Cache<ReadOnlyCollection<T>>._formatter = enumerableFormatter;
        Cache<Collection<T>>._formatter = enumerableFormatter;
        Cache<List<T>>._formatter = enumerableFormatter;
        Cache<List<T?>>._formatter = enumerableFormatter;
        Cache<LinkedList<T>>._formatter = enumerableFormatter;
        Cache<HashSet<T>>._formatter = enumerableFormatter;
        Cache<SortedSet<T>>._formatter = enumerableFormatter;
        Cache<Queue<T>>._formatter = enumerableFormatter;
        Cache<Stack<T>>._formatter = enumerableFormatter;
        Cache<ReadOnlyObservableCollection<T>>._formatter = enumerableFormatter;
        Cache<ObservableCollection<T>>._formatter = enumerableFormatter;
    }

    public static void Register(IRedisValueFormatter formatter)
    {
        _default = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public static IRedisValueFormatter<T>? GetFormatter<T>() => Cache<T>._formatter;
}