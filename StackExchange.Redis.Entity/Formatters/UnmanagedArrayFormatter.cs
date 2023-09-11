using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public class UnmanagedArrayFormatter<T> : IRedisValueFormatter<T[]> where T : unmanaged
{
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

            ref byte bytes = ref MemoryMarshal.GetReference(span);

            for (int i = 0, b = 0; i < value.Length; i++, b += size)
            {
                value[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b));
            }
        }
    }

    public void Deserialize(in RedisValue redisValue, ref T?[]? value)
    {
        throw new NotImplementedException();
    }

    public RedisValue Serialize(in T[]? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Length];

        for (int i = 0, b = 0; i < value.Length; i++, b+= size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    public RedisValue Serialize(in T?[]? value)
    {
        throw new NotImplementedException();
    }
}