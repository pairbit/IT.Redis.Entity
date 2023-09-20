using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace StackExchange.Redis.Entity.Formatters;

public class StringArrayFormatter : IRedisValueFormatter<string?[]>
{
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
            if (span.Length < 5) throw Ex.InvalidMinLength(typeof(string?[]), span.Length, 5);

            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            //[4] array.length
            var length = Unsafe.ReadUnaligned<int>(ref spanRef);

            if (value == null || value.Length != length) value = new string?[length];

            var offset = 4;

            for (int i = 0; i < length; i++)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                if (strlen == int.MaxValue)
                {
                    value[i] = null;
                    offset += 4;
                }
                else if (strlen == 0)
                {
                    value[i] = string.Empty;
                    offset += 4;
                }
                else
                {
                    value[i] = _encoding.GetString(span.Slice(offset + 4, strlen));

                    offset += 4 + strlen;
                }
            }
        }
    }

    public RedisValue Serialize(in string?[]? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var length = 4;
        for (int i = 0; i < value.Length; i++)
        {
            var str = value[i];
            length += 4 + (str == null || str.Length == 0 ? 0 : _encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        //[4] array.length
        Unsafe.WriteUnaligned(ref bytes[0], value.Length);

        var offset = 4;
        var span = bytes.AsSpan();

        for (int i = 0; i < value.Length; i++)
        {
            var str = value[i];

            if (str == null)
            {
                //[4] string.length
                Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                offset += 4;
            }
            else if (str.Length == 0)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], 0);

                offset += 4;
            }
            else
            {
                //[string.length] string
                var written = _encoding.GetBytes(str, span.Slice(offset + 4));

                //[4] string.length
                Unsafe.WriteUnaligned(ref bytes[offset], written);

                offset += 4 + written;
            }
        }

        return bytes;
    }
}