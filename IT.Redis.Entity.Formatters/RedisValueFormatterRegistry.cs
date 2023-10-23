using IT.Collections.Factory;
using IT.Collections.Factory.Generic;
using IT.Redis.Entity.Formatters;
using IT.Redis.Entity.Internal;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace IT.Redis.Entity;

public class RedisValueFormatterRegistry : IRedisValueFormatter
{
    public static readonly RedisValueFormatterRegistry Default = new();
    public static readonly IEnumerableFactoryRegistry EnumerableFactoryRegistry = 
        new ConcurrentEnumerableFactoryRegistry().RegisterFactoriesDefault();

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

    static readonly Type StringDictionaryFormatterType = typeof(StringDictionaryFormatter<>);
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
    

    static IRedisValueFormatter _defaultFormatter = new RedisValueFormatterNotRegistered();

    internal static IRedisValueFormatter DefaultFormatter => _defaultFormatter;

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
        Register(PairStringArrayFormatter.Default);
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
                return Activator.CreateInstance(UnmanagedArrayFormatterType.MakeGenericType(elementType.GetNullableUnderlyingType()));
            }
        }
        else if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();

            //if (genericType.IsUnmanaged() && _unmanagedGenericFormatterTypes.TryGetValue(genericType, out var genericFormatterType))
            //    return genericFormatterType.MakeGenericType(type.GetGenericArguments());

            //if (_genericFormatterTypes.TryGetValue(genericType, out genericFormatterType))
            //    return genericFormatterType.MakeGenericType(type.GetGenericArguments());

            if (EnumerableFactoryRegistry.TryGetValue(genericType, out var factory) && factory is IEnumerableFactory enumerableFactory)
            {
                var args = type.GetGenericArguments();
                var elementType = args[0];

                if (elementType.IsUnmanaged())
                {
                    var formatterType = elementType.IsNullable()
                        ? UnmanagedEnumerableNullableFormatterType.MakeGenericType(type, elementType.GetGenericArguments()[0])
                        : UnmanagedEnumerableFormatterType.MakeGenericType(type, elementType);

                    var genericFactory = Activator.CreateInstance(EnumerableFactoryProxyType.MakeGenericType(type, elementType), enumerableFactory);

                    return Activator.CreateInstance(formatterType, genericFactory);
                }

                if (elementType == typeof(string))
                {
                    var genericFactory = Activator.CreateInstance(EnumerableFactoryProxyType.MakeGenericType(type, elementType), enumerableFactory);

                    return Activator.CreateInstance(StringEnumerableFormatterType.MakeGenericType(type), genericFactory);
                }

                if (elementType == typeof(KeyValuePair<string?, string?>))
                {
                    var genericFactory = Activator.CreateInstance(EnumerableFactoryProxyType.MakeGenericType(type, elementType), enumerableFactory);

                    return Activator.CreateInstance(StringDictionaryFormatterType.MakeGenericType(type), genericFactory);
                }
            }

            if (EnumerableFactoryRegistry.TryGetValue(genericType, out factory) && factory is IEnumerableKeyValueFactory dictionaryFactory)
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

                if (keyType == typeof(string) && valueType == typeof(string))
                {
                    var genericFactory = Activator.CreateInstance(DictionaryFactoryProxyType.MakeGenericType(type, keyType, valueType), dictionaryFactory);

                    return Activator.CreateInstance(StringDictionaryFormatterType.MakeGenericType(type), genericFactory);
                }
            }
        }

        return null;
    }
}