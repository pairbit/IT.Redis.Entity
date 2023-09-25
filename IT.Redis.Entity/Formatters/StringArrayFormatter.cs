using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace IT.Redis.Entity.Formatters;

public class StringArrayFormatter : IRedisValueFormatter<string?[]>
{
    private const int Size = 4;
    private const int MinLength = Size * 2;
    public static readonly StringArrayFormatter Default = new();

    private readonly Encoding _encoding;

    public StringArrayFormatter()
    {
        _encoding = Encoding.UTF8;
    }

    public StringArrayFormatter(Encoding encoding)
    {
        _encoding = encoding;
    }

    public void Deserialize(in RedisValue redisValue, ref string?[]? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Array.Empty<string?>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            if (span.Length < MinLength) throw Ex.InvalidMinLength(typeof(string?[]), span.Length, MinLength);

            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            //[Size] array.length
            var length = Unsafe.ReadUnaligned<int>(ref spanRef);

            if (value == null || value.Length != length) value = new string?[length];

            span = span.Slice(Size * value.Length + Size);

            for (int i = 0, b = Size; i < value.Length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) value[i] = null;
                else if (strlen == 0) value[i] = string.Empty;
                else
                {
                    value[i] = _encoding.GetString(span.Slice(0, strlen));
                    span = span.Slice(strlen);
                }
            }
        }
    }

    public RedisValue Serialize(in string?[]? value) => value == null ? RedisValues.Zero : StringEnumerableFormatter.Serialize(_encoding, value);
}