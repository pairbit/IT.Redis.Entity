using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;

namespace IT.Redis.Entity.Formatters;

public class UnmanagedEnumerableFormatter<TEnumerable, T> : IRedisValueFormatter<TEnumerable>
    where TEnumerable : IEnumerable<T>
    where T : unmanaged
{
    private readonly IEnumerableFactory<TEnumerable, T> _factory;

    public UnmanagedEnumerableFormatter(IEnumerableFactory<TEnumerable, T> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public UnmanagedEnumerableFormatter(EnumerableFactory<TEnumerable, T> factory)
    {
        _factory = new EnumerableFactoryDelegate<TEnumerable, T>(factory ?? throw new ArgumentNullException(nameof(factory)));
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
            var length = span.Length / size;

            if (value != null)
            {
                var enumerable = (IEnumerable<T>)value;

                if (UnmanagedEnumerableFormatter.Deserialize(ref enumerable, in span, size, length))
                {
                    value = (TEnumerable)enumerable;
                    return;
                }
            }

            value = _factory.New(length, in memory, UnmanagedEnumerableFormatter.Build);
        }
    }

    public RedisValue Serialize(in TEnumerable? value) => UnmanagedEnumerableFormatter.Serialize(value);
}