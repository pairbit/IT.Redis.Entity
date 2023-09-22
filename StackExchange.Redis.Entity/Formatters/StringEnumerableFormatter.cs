using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace StackExchange.Redis.Entity.Formatters;

public class StringEnumerableFormatter<TEnumerable> : IRedisValueFormatter<TEnumerable> where TEnumerable : IEnumerable<string?>
{
    private readonly Encoding _encoding = Encoding.UTF8;
    private readonly IEnumerableFactory<TEnumerable, string?> _factory;

    public StringEnumerableFormatter(IEnumerableFactory<TEnumerable, string?> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public StringEnumerableFormatter(EnumerableFactory<TEnumerable, string?> factory)
    {
        _factory = new EnumerableFactoryDelegate<TEnumerable, string?>(factory ?? throw new ArgumentNullException(nameof(factory)));
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
            if (span.Length < StringEnumerableFormatter.MinLength) 
                throw Ex.InvalidMinLength(typeof(TEnumerable), span.Length, StringEnumerableFormatter.MinLength);

            var length = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span));

            var state = new BuildState(memory, _encoding, length);

            if (value != null)
            {
                var enumerable = (IEnumerable<string?>)value;

                if (StringEnumerableFormatter.Deserialize(ref enumerable, in state))
                {
                    value = (TEnumerable)enumerable;
                    return;
                }
            }

            value = _factory.New(length, in state, StringEnumerableFormatter.Build);
        }
    }

    public RedisValue Serialize(in TEnumerable? value) => StringEnumerableFormatter.Serialize(_encoding, value);
}