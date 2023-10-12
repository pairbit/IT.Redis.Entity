using IT.Collections.Factory.Generic;
using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;

namespace IT.Redis.Entity.Formatters;

public class UnmanagedEnumerableNullableFormatter<TEnumerable, T> : IRedisValueFormatter<TEnumerable>
    where TEnumerable : IEnumerable<T?>
    where T : unmanaged
{
    private readonly IEnumerableFactory<TEnumerable, T?> _factory;

    public UnmanagedEnumerableNullableFormatter(IEnumerableFactory<TEnumerable, T?> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public UnmanagedEnumerableNullableFormatter(EnumerableFactory<TEnumerable, T?> factory, Action<TEnumerable, T?> add, bool reverse)
    {
        _factory = new EnumerableFactoryDelegate<TEnumerable, T?>(factory, add, reverse);
    }

    public void Deserialize(in RedisValue redisValue, ref TEnumerable? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = default;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = _factory.Empty();
        }
        else
        {
            var memory = (ReadOnlyMemory<byte>)redisValue;
            var span = memory.Span;
            var size = Unsafe.SizeOf<T>();
            var length = (int)(((long)span.Length << 3) / ((size << 3) + 1));

            if (value != null)
            {
                var enumerable = (IEnumerable<T?>)value;

                if (UnmanagedEnumerableNullableFormatter.Deserialize(ref enumerable, in span, size, length))
                {
                    value = (TEnumerable)enumerable;
                    return;
                }
            }

            value = _factory.New(length, UnmanagedEnumerableNullableFormatter.Build, in memory);
        }
    }

    public RedisValue Serialize(in TEnumerable? value) => UnmanagedEnumerableNullableFormatter.Serialize(value);
}