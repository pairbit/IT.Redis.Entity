using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace IT.Redis.Entity.Formatters;

public class StringDictionaryFormatter<TDictionary> : IRedisValueFormatter<TDictionary>
    where TDictionary : IEnumerable<KeyValuePair<string?, string?>>
{
    private readonly Encoding _encoding = Encoding.UTF8;
    private readonly IEnumerableFactory<TDictionary, KeyValuePair<string?, string?>> _factory;

    public StringDictionaryFormatter(IEnumerableFactory<TDictionary, KeyValuePair<string?, string?>> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public StringDictionaryFormatter(EnumerableFactory<TDictionary, KeyValuePair<string?, string?>> factory)
    {
        _factory = new EnumerableFactoryDelegate<TDictionary, KeyValuePair<string?, string?>>(factory ?? throw new ArgumentNullException(nameof(factory)));
    }

    public void Deserialize(in RedisValue redisValue, ref TDictionary? value)
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
            if (span.Length < StringDictionaryFormatter.MinLength)
                throw Ex.InvalidMinLength(typeof(TDictionary), span.Length, StringDictionaryFormatter.MinLength);

            var length = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span));

            var state = new BuildState(memory, _encoding, length);

            if (value != null)
            {
                var enumerable = (IEnumerable<KeyValuePair<string?, string?>>)value;

                if (StringDictionaryFormatter.Deserialize(ref enumerable, in state))
                {
                    value = (TDictionary)enumerable;
                    return;
                }
            }

            value = _factory.New(length, in state, StringDictionaryFormatter.Build);
        }
    }

    public RedisValue Serialize(in TDictionary? value) => StringDictionaryFormatter.Serialize(_encoding, value);
}