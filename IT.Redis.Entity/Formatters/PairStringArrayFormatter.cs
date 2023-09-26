using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace IT.Redis.Entity.Formatters;

public class PairStringArrayFormatter : IRedisValueFormatter<KeyValuePair<string?, string?>[]>
{
    private const int Size = 4;
    internal const int DoubleSize = Size * 2;
    internal const int MinLength = Size * 3;

    public static readonly PairStringArrayFormatter Default = new();

    private readonly Encoding _encoding;

    public PairStringArrayFormatter()
    {
        _encoding = Encoding.UTF8;
    }

    public PairStringArrayFormatter(Encoding encoding)
    {
        _encoding = encoding;
    }

    public void Deserialize(in RedisValue redisValue, ref KeyValuePair<string?, string?>[]? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Array.Empty<KeyValuePair<string?, string?>>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;

            if (span.Length < MinLength) throw Ex.InvalidMinLength(typeof(KeyValuePair<string?, string?>[]), span.Length, MinLength);

            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            var length = Unsafe.ReadUnaligned<int>(ref spanRef);

            if (value == null || value.Length != length) value = new KeyValuePair<string?, string?>[length];

            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < value.Length; i++, b += DoubleSize)
            {
                value[i] = UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, _encoding);
            }
        }
    }

    public RedisValue Serialize(in KeyValuePair<string?, string?>[]? value)
        => value == null ? RedisValues.Zero : StringDictionaryFormatter.SerializeArray(_encoding, value);
}