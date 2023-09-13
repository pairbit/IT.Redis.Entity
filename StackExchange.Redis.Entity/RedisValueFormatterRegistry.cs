using StackExchange.Redis.Entity.Formatters;
using StackExchange.Redis.Entity.Internal;
using System.Collections.ObjectModel;

namespace StackExchange.Redis.Entity;

public static class RedisValueFormatterRegistry
{
    static class Check<T>
    {
        public static bool _registered;
    }

    static class Cache<T>
    {
        public static IRedisValueFormatter<T>? _formatter;

        static Cache()
        {
            if (Check<T>._registered) return;

            var formatter = _resolver.GetFormatter<T>();

            if (formatter != null)
            {
                Cache<T>._formatter = formatter;
                Check<T>._registered = true;
            }
        }
    }

    private static IRedisValueFormatterResolver _resolver = new RedisValueFormatterResolver();
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
        if (formatter == null) throw new ArgumentNullException(nameof(formatter));

        Check<T>._registered = true;
        Cache<T>._formatter = formatter;
    }

    public static void Register<T>(IStructFormatter<T> formatter) where T : struct
    {
        Register((IRedisValueFormatter<T>)formatter);
        Register((IRedisValueFormatter<T?>)formatter);
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

        Register<T[]>(enumerableFormatter);
        Register<T?[]>(enumerableFormatter);
        Register<IEnumerable<T>>(enumerableFormatter);
        Register<IReadOnlyCollection<T>>(enumerableFormatter);
        Register<IReadOnlyList<T>>(enumerableFormatter);
        Register<IReadOnlyList<T?>>(enumerableFormatter);
        Register<IReadOnlySet<T>>(enumerableFormatter);
        Register<ICollection<T>>(enumerableFormatter);
        Register<IList<T>>(enumerableFormatter);
        Register<IList<T?>>(enumerableFormatter);
        Register<ISet<T>>(enumerableFormatter);
        Register<ReadOnlyCollection<T>>(enumerableFormatter);
        Register<Collection<T>>(enumerableFormatter);
        Register<List<T>>(enumerableFormatter);
        Register<List<T?>>(enumerableFormatter);
        Register<LinkedList<T>>(enumerableFormatter);
        Register<HashSet<T>>(enumerableFormatter);
        Register<SortedSet<T>>(enumerableFormatter);
        Register<Queue<T>>(enumerableFormatter);
        Register<Stack<T>>(enumerableFormatter);
        Register<ReadOnlyObservableCollection<T>>(enumerableFormatter);
        Register<ObservableCollection<T>>(enumerableFormatter);
    }

    public static void Register(IRedisValueFormatter formatter)
    {
        _default = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public static IRedisValueFormatter<T>? GetFormatter<T>() => Cache<T>._formatter;

    public static bool IsRegistered<T>() => Check<T>._registered;
}