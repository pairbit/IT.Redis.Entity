using StackExchange.Redis.Entity.Factories;
using StackExchange.Redis.Entity.Formatters;
using StackExchange.Redis.Entity.Internal;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace StackExchange.Redis.Entity;

public class RedisValueFormatterRegistry : IRedisValueFormatter
{
    public static readonly RedisValueFormatterRegistry Default = new();

    static class Check<T>
    {
        public static bool _registered;
    }

    static class Cache<T>
    {
        public static IRedisValueFormatter<T> _formatter = null!;

        static Cache()
        {
            if (Check<T>._registered) return;

            var type = typeof(T);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            var isUnmanagedType = !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
            var isUnmanagedType = type.IsUnmanaged();
#endif
            var formatter = GetFormatter(type, isUnmanagedType) as IRedisValueFormatter<T>;

            if (formatter == null)
            {
                Cache<T>._formatter = (IRedisValueFormatter<T>)Activator.CreateInstance(RedisValueFormatterDefaultType.MakeGenericType(type))!;
            }
            else
            {
                Cache<T>._formatter = formatter;
                Check<T>._registered = true;
            }
        }
    }

    static readonly Type RedisValueFormatterDefaultType = typeof(RedisValueFormatterDefault<>);
    static readonly Type EnumFormatterType = typeof(EnumFormatter<,>);
    static readonly Type UnmanagedFormatterType = typeof(UnmanagedFormatter<>);

    static readonly Type StringEnumerableFormatterType = typeof(StringEnumerableFormatter<>);
    static readonly Type UnmanagedArrayFormatterType = typeof(UnmanagedArrayFormatter<>);
    static readonly Type UnmanagedDictionaryFormatterType = typeof(UnmanagedDictionaryFormatter<,,>);
    static readonly Type UnmanagedDictionaryNullableFormatterType = typeof(UnmanagedDictionaryNullableFormatter<,,>);
    static readonly Type UnmanagedEnumerableFormatterType = typeof(UnmanagedEnumerableFormatter<,>);
    static readonly Type UnmanagedEnumerableNullableFormatterType = typeof(UnmanagedEnumerableNullableFormatter<,>);
    static readonly Type EnumerableFactoryProxyType = typeof(EnumerableFactoryProxy<,>);
    static readonly Type DictionaryFactoryProxyType = typeof(DictionaryFactoryProxy<,,>);

    static readonly ConcurrentDictionary<Type, Type> _genericFormatterTypes = new();
    static readonly ConcurrentDictionary<Type, Type> _unmanagedGenericFormatterTypes = new();
    static readonly ConcurrentDictionary<Type, IEnumerableFactory> _unmanagedGenericEnumerableTypes = new();
    static readonly ConcurrentDictionary<Type, IDictionaryFactory> _unmanagedGenericDictionaryTypes = new();

    static IRedisValueFormatter _defaultFormatter = new RedisValueFormatterNotRegistered();
    static CompressionOptions _compressionOptions = CompressionOptions.Default;

    internal static IRedisValueFormatter DefaultFormatter => _defaultFormatter;

    public static CompressionOptions CompressionOptions => _compressionOptions;

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
#if NET6_0_OR_GREATER
        Register(DateOnlyFormatter.Default);
        Register(TimeOnlyFormatter.Default);
#endif
        Register(VersionFormatter.Default);
        Register(UriFormatter.Default);

        Register(StringFormatter.Default);
        Register(ByteArrayFormatter.Default);
        Register(BitArrayFormatter.Default);

        Register(StringArrayFormatter.Default);

        //RegisterEnumerableFactory(ArrayFactory.Default, typeof(IEnumerable<>));
        RegisterEnumerableFactory(ListFactory.Default, typeof(IList<>));
        RegisterEnumerableFactory(LinkedListFactory.Default, typeof(ICollection<>), typeof(IEnumerable<>));
        RegisterEnumerableFactory(HashSetFactory.Default, typeof(ISet<>));
        RegisterEnumerableFactory(SortedSetFactory.Default);
        RegisterEnumerableFactory(StackFactory.Default);
        RegisterEnumerableFactory(QueueFactory.Default);
        RegisterEnumerableFactory(CollectionFactory.Default);
        RegisterEnumerableFactory(ObservableCollectionFactory.Default);
        RegisterEnumerableFactory(ReadOnlyObservableCollectionFactory.Default);

        RegisterEnumerableFactory(ReadOnlyListFactory.Default, typeof(IReadOnlyList<>));
        RegisterEnumerableFactory(ReadOnlyLinkedListFactory.Default, typeof(IReadOnlyCollection<>));
#if NET6_0_OR_GREATER
        RegisterEnumerableFactory(ReadOnlyHashSetFactory.Default, typeof(IReadOnlySet<>));
#endif
#if NETCOREAPP3_1_OR_GREATER
        RegisterEnumerableFactory(ImmutableArrayFactory.Default);
        RegisterEnumerableFactory(ImmutableListFactory.Default, typeof(System.Collections.Immutable.IImmutableList<>));
        RegisterEnumerableFactory(ImmutableHashSetFactory.Default, typeof(System.Collections.Immutable.IImmutableSet<>));
        RegisterEnumerableFactory(ImmutableSortedSetFactory.Default);
        RegisterEnumerableFactory(ImmutableStackFactory.Default, typeof(System.Collections.Immutable.IImmutableStack<>));
        RegisterEnumerableFactory(ImmutableQueueFactory.Default, typeof(System.Collections.Immutable.IImmutableQueue<>));
        RegisterDictionaryFactory(ImmutableDictionaryFactory.Default, typeof(System.Collections.Immutable.IImmutableDictionary<,>));
        RegisterDictionaryFactory(ImmutableSortedDictionaryFactory.Default);
#endif
        RegisterDictionaryFactory(DictionaryFactory.Default, typeof(IDictionary<,>));
        RegisterDictionaryFactory(ReadOnlyDictionaryFactory.Default, typeof(IReadOnlyDictionary<,>));
        RegisterDictionaryFactory(SortedDictionaryFactory.Default);
        RegisterDictionaryFactory(SortedListFactory.Default);

        RegisterDictionaryFactory(ConcurrentDictionaryFactory.Default);
        RegisterEnumerableFactory(ConcurrentBagFactory.Default, typeof(IProducerConsumerCollection<>));
        RegisterEnumerableFactory(ConcurrentQueueFactory.Default);
        RegisterEnumerableFactory(ConcurrentStackFactory.Default);
        RegisterEnumerableFactory(BlockingCollectionFactory.Default);
    }

    private RedisValueFormatterRegistry() { }

    public void Deserialize<T>(in RedisValue redisValue, ref T? value) => GetFormatter<T>().Deserialize(in redisValue, ref value);

    public RedisValue Serialize<T>(in T? value) => GetFormatter<T>().Serialize(in value);

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

    public static void RegisterDefault(IRedisValueFormatter formatter)
    {
        _defaultFormatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public static IRedisValueFormatter<T> GetFormatter<T>() => Cache<T>._formatter;

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

    public static void RegisterEnumerableFactory(IEnumerableFactory factory, params Type[] genericTypes)
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));

        var enumerable = factory.Empty<int>();

        if (enumerable == null) throw new ArgumentException("Factory Error", nameof(factory));

        var enumerableType = enumerable.GetType();

        Type baseType;

        if (enumerableType.IsArray)
        {
            baseType = typeof(Array);
        }
        else
        {
            if (!enumerableType.IsGenericType) throw new ArgumentException($"Registered type '{enumerableType.FullName}' is not generic type", nameof(factory));

            baseType = enumerableType.GetGenericTypeDefinition();

            _unmanagedGenericEnumerableTypes[baseType] = factory;
        }

        if (genericTypes != null && genericTypes.Length > 0)
        {
            for (int i = 0; i < genericTypes.Length; i++)
            {
                var genericTypeBase = genericTypes[i];

                if (!genericTypeBase.IsGenericType)
                    throw new ArgumentException($"Registered type '{genericTypeBase.FullName}' is not generic type", nameof(genericTypes));

                if (!genericTypeBase.MakeGenericType(typeof(int)).IsAssignableFrom(enumerableType))
                    throw new ArgumentException($"Registered type '{genericTypeBase.FullName}' is not assignable from type '{baseType.FullName}'", nameof(genericTypes));

                _unmanagedGenericEnumerableTypes[genericTypeBase] = factory;
            }
        }
    }

    public static void RegisterDictionaryFactory(IDictionaryFactory factory, params Type[] genericTypes)
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));

        var enumerable = factory.Empty<int, int>();

        if (enumerable == null) throw new ArgumentException("Factory Error", nameof(factory));

        var enumerableType = enumerable.GetType();

        if (!enumerableType.IsGenericType) throw new ArgumentException($"Registered type '{enumerableType.FullName}' is not generic type", nameof(factory));

        var baseType = enumerableType.GetGenericTypeDefinition();

        _unmanagedGenericDictionaryTypes[baseType] = factory;

        if (genericTypes != null && genericTypes.Length > 0)
        {
            for (int i = 0; i < genericTypes.Length; i++)
            {
                var genericTypeBase = genericTypes[i];

                if (!genericTypeBase.IsGenericType)
                    throw new ArgumentException($"Registered type '{genericTypeBase.FullName}' is not generic type", nameof(genericTypes));

                if (!genericTypeBase.MakeGenericType(typeof(int), typeof(int)).IsAssignableFrom(enumerableType))
                    throw new ArgumentException($"Registered type '{genericTypeBase.FullName}' is not assignable from type '{baseType.FullName}'", nameof(genericTypes));

                _unmanagedGenericDictionaryTypes[genericTypeBase] = factory;
            }
        }
    }

    private static object? GetFormatter(Type type, bool isUnmanagedType)
    {
        if (isUnmanagedType)
        {
            type = type.GetNullableUnderlyingType();

            var formatterType = type.IsEnum
                ? EnumFormatterType.MakeGenericType(type, type.GetEnumUnderlyingType())
                : UnmanagedFormatterType.MakeGenericType(type);

            return Activator.CreateInstance(formatterType);
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        if (type.IsArray && type.IsSZArray)
#else
        if (type.IsArray && type.GetArrayRank() == 1)
#endif
        {
            var elementType = type.GetElementType();
            if (elementType != null && elementType.IsUnmanaged())
            {
                return Activator.CreateInstance(UnmanagedArrayFormatterType.MakeGenericType(elementType.GetNullableUnderlyingType()), _compressionOptions);
            }
        }
        else if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();

            //if (genericType.IsUnmanaged() && _unmanagedGenericFormatterTypes.TryGetValue(genericType, out var genericFormatterType))
            //    return genericFormatterType.MakeGenericType(type.GetGenericArguments());

            //if (_genericFormatterTypes.TryGetValue(genericType, out genericFormatterType))
            //    return genericFormatterType.MakeGenericType(type.GetGenericArguments());

            if (_unmanagedGenericEnumerableTypes.TryGetValue(genericType, out var factory))
            {
                var args = type.GetGenericArguments();
                var elementType = args[0];

                if (elementType.IsUnmanaged())
                {
                    var formatterType = elementType.IsNullable()
                        ? UnmanagedEnumerableNullableFormatterType.MakeGenericType(type, elementType.GetGenericArguments()[0])
                        : UnmanagedEnumerableFormatterType.MakeGenericType(type, elementType);

                    var genericFactory = Activator.CreateInstance(EnumerableFactoryProxyType.MakeGenericType(type, elementType), factory);

                    return Activator.CreateInstance(formatterType, genericFactory);
                }

                if (elementType == typeof(string))
                {
                    var genericFactory = Activator.CreateInstance(EnumerableFactoryProxyType.MakeGenericType(type, elementType), factory);

                    return Activator.CreateInstance(StringEnumerableFormatterType.MakeGenericType(type), genericFactory);
                }
            }

            if (_unmanagedGenericDictionaryTypes.TryGetValue(genericType, out var dictionaryFactory))
            {
                var args = type.GetGenericArguments();

                if (args.Length != 2) throw new InvalidOperationException();

                var keyType = args[0];
                var valueType = args[1];

                if (keyType.IsUnmanaged() && !keyType.IsNullable() && valueType.IsUnmanaged())
                {
                    var formatterType = valueType.IsNullable()
                        ? UnmanagedDictionaryNullableFormatterType.MakeGenericType(type, keyType, valueType.GetGenericArguments()[0])
                        : UnmanagedDictionaryFormatterType.MakeGenericType(type, keyType, valueType);

                    var genericFactory = Activator.CreateInstance(DictionaryFactoryProxyType.MakeGenericType(type, keyType, valueType), dictionaryFactory);

                    return Activator.CreateInstance(formatterType, genericFactory);
                }
            }
        }

        return null;
    }
}