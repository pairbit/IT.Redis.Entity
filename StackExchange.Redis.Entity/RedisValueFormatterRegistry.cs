using StackExchange.Redis.Entity.Formatters;
using StackExchange.Redis.Entity.Internal;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

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

            var formatter = GetFormatter(typeof(T), !RuntimeHelpers.IsReferenceOrContainsReferences<T>()) as IRedisValueFormatter<T>;

            if (formatter != null)
            {
                Cache<T>._formatter = formatter;
                Check<T>._registered = true;
            }
        }
    }

    static readonly Type NullableType = typeof(Nullable<>);
    static readonly Type UnmanagedFormatterType = typeof(UnmanagedFormatter<>);

    private static readonly Type UnmanagedEnumerableFormatterType = typeof(UnmanagedEnumerableFormatter<,>);
    private static readonly Type UnmanagedEnumerableNullableFormatterType = typeof(UnmanagedEnumerableNullableFormatter<,>);
    private static readonly Type EnumerableType = typeof(IEnumerable<>);

    static readonly ConcurrentDictionary<Type, Type> _genericFormatterTypes = new();
    static readonly ConcurrentDictionary<Type, Type> _unmanagedGenericFormatterTypes = new();
    static readonly ConcurrentBag<Type> _unmanagedGenericEnumerableTypes = new()
    {
        EnumerableType,
        typeof(IReadOnlyList<>),
        typeof(IReadOnlyCollection<>),
        typeof(IReadOnlySet<>),
        typeof(IList<>),
        typeof(ICollection<>),
        typeof(ISet<>),
        typeof(List<>),
        typeof(HashSet<>),
        typeof(SortedSet<>),
        typeof(Stack<>),
    };

    static IRedisValueFormatter _default = new RedisValueFormatterNotRegistered();
    static IEnumerableFactory _enumerableFactory = new EnumerableFactory();

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

        //RegisterUnmanagedEnumerable<Int32>();
        //RegisterUnmanagedEnumerable<Guid>();
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

    public static void Register(IRedisValueFormatter formatter)
    {
        _default = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public static IRedisValueFormatter<T>? GetFormatter<T>() => Cache<T>._formatter;

    public static bool IsRegistered<T>() => Check<T>._registered;

    public static void RegisterGenericType(Type genericType, Type genericFormatterType)
    {
        if (!genericType.IsGenericType) throw new ArgumentException($"Registered type '{genericType.FullName}' is not generic type", nameof(genericType));
        if (!genericFormatterType.IsGenericType) throw new ArgumentException($"Registered formatter type '{genericFormatterType.FullName}' is not generic type", nameof(genericFormatterType));

        _genericFormatterTypes[genericType] = genericFormatterType;
    }

    public static void RegisterUnmanagedGenericType(Type genericType, Type genericFormatterType)
    {
        if (!genericType.IsGenericType) throw new ArgumentException($"Registered type '{genericType.FullName}' is not generic type", nameof(genericType));
        if (!genericFormatterType.IsGenericType) throw new ArgumentException($"Registered formatter type '{genericFormatterType.FullName}' is not generic type", nameof(genericFormatterType));

        _unmanagedGenericFormatterTypes[genericType] = genericFormatterType;
    }

    public static void RegisterUnmanagedEnumerableGenericType(Type genericType)
    {
        if (!genericType.IsGenericType) throw new ArgumentException($"Registered type '{genericType.FullName}' is not generic type", nameof(genericType));

        _unmanagedGenericEnumerableTypes.Add(genericType);
    }

    private static object? GetFormatter(Type type, bool isUnmanagedType)
    {
        if (isUnmanagedType) return Activator.CreateInstance(UnmanagedFormatterType.MakeGenericType(
            type.IsGenericType && type.GetGenericTypeDefinition().Equals(NullableType)
            ? type.GetGenericArguments()[0] : type));

        if (type.IsArray && type.IsSZArray) return GetUnmanagedEnumerableFormatter(type, type.GetElementType());
        else if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();

            //if (genericType.IsUnmanaged() && _unmanagedGenericFormatterTypes.TryGetValue(genericType, out var genericFormatterType))
            //    return genericFormatterType.MakeGenericType(type.GetGenericArguments());

            //if (_genericFormatterTypes.TryGetValue(genericType, out genericFormatterType))
            //    return genericFormatterType.MakeGenericType(type.GetGenericArguments());

            if (_unmanagedGenericEnumerableTypes.Contains(genericType))
                return GetUnmanagedEnumerableFormatter(type, type.GetGenericArguments()[0]);
        }

        return null;

        //else
        //{
        //    var iTypes = type.GetInterfaces();
        //    for (int i = 0; i < iTypes.Length; i++)
        //    {
        //        var iType = iTypes[i];
        //        if (iType.IsGenericType && iType.GetGenericTypeDefinition().Equals(IEnumerableType))
        //        {
        //            var genericArgument = iType.GetGenericArguments()[0];
        //            if (genericArgument.IsUnmanaged())
        //            {
        //                formatterType = UnmanagedIEnumerableFormatterType.MakeGenericType(type, genericArgument);
        //                break;
        //            }
        //        }
        //    }
        //}
    }

    private static object? GetUnmanagedEnumerableFormatter(Type type, Type? elementType)
    {
        if (elementType == null || !elementType.IsUnmanaged()) return null;

        var formatterType = elementType.IsGenericType && elementType.GetGenericTypeDefinition().Equals(NullableType)
            ? UnmanagedEnumerableNullableFormatterType.MakeGenericType(type, elementType.GetGenericArguments()[0])
            : UnmanagedEnumerableFormatterType.MakeGenericType(type, elementType);

        return Activator.CreateInstance(formatterType, _enumerableFactory);
    }
}