using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace StackExchange.Redis.Entity.Formatters;

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

            var offset = Size;

            for (int i = 0; i < length; i++)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                if (strlen == int.MaxValue)
                {
                    value[i] = null;
                    offset += Size;
                }
                else if (strlen == 0)
                {
                    value[i] = string.Empty;
                    offset += Size;
                }
                else
                {
                    value[i] = _encoding.GetString(span.Slice(offset + Size, strlen));

                    offset += Size + strlen;
                }
            }
        }
    }

    public RedisValue Serialize(in string?[]? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Length; i++)
        {
            var str = value[i];
            length += Size + (str == null || str.Length == 0 ? 0 : _encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        //[Size] array.length
        Unsafe.WriteUnaligned(ref bytes[0], value.Length);

        var offset = Size;
        var span = bytes.AsSpan();

        for (int i = 0; i < value.Length; i++)
        {
            var str = value[i];

            if (str == null)
            {
                //[Size] string.length
                Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                offset += Size;
            }
            else if (str.Length == 0)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], 0);

                offset += Size;
            }
            else
            {
                //[string.length] string
                var written = _encoding.GetBytes(str, span.Slice(offset + Size));

                //[Size] string.length
                Unsafe.WriteUnaligned(ref bytes[offset], written);

                offset += Size + written;
            }
        }

        return bytes;
    }
}