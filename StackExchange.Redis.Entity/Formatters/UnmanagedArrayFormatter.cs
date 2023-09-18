using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public class UnmanagedArrayFormatter<T> : IRedisValueFormatter<T[]>, IRedisValueFormatter<T?[]> where T : unmanaged
{
    public void Deserialize(in RedisValue redisValue, ref T?[]? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Array.Empty<T?>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T>();
            //var len = int.MaxValue;
            var length = (int)(((long)span.Length << 3) / ((size << 3) + 1));//span.Length * 8 / ((size * 8) + 1)

            if (value == null || value.Length != length) value = new T?[length];

            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            var iBits = 0;
            var iBytes = size * length;
            var bits = span[iBytes];
            int i = 0, b = 0;
            do
            {
                if ((bits & (1 << iBits)) == 0) value[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));

                if (++i == value.Length) break;

                b += size;

                if (++iBits == 8)
                {
                    bits = span[++iBytes];
                    iBits = 0;
                }
            } while (true);

            var value2 = Util.DeserializeSlow<T>(redisValue);

            if (!value.SequenceEqual(value2!)) throw new InvalidOperationException();
        }
    }

    public void Deserialize(in RedisValue redisValue, ref T[]? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Array.Empty<T>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T>();
            var length = span.Length / size;

            if (value == null || value.Length != length) value = new T[length];

            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            for (int i = 0, b = 0; i < value.Length; i++, b += size)
            {
                value[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));
            }
        }
    }

    public RedisValue Serialize(in T?[]? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var sizeValue = size * value.Length;
        var bytes = new byte[sizeValue + (value.Length - 1) / 8 + 1];

        byte bits = 0;
        var iBits = 0;
        var iBytes = sizeValue;

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            var item = value[i];
            if (item == null)
            {
                bits = (byte)(bits | (1 << iBits));
            }
            else
            {
                Unsafe.WriteUnaligned(ref bytes[b], item.Value);
            }
            if (++iBits == 8)
            {
                bytes[iBytes++] = bits;
                iBits = 0;
                bits = 0;
            }
        }

        if (iBits > 0) bytes[iBytes] = bits;

        var bytesRedis = (RedisValue)bytes;

        var bytesRedis2 = Util.SerializeSlow(value);

        if (bytesRedis != bytesRedis2) throw new InvalidOperationException();

        return bytes;
    }

    public RedisValue Serialize(in T[]? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Length];

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }
}