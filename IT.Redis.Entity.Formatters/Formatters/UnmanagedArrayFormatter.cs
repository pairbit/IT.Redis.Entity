using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IT.Redis.Entity.Formatters;

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

            //if (Util.IsCompressed(span))
            //{
            //    var decompressed = Util.Decompress((byte[])redisValue!);
            //    span = decompressed;
            //}

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

            //var value2 = Util.DeserializeSlow<T>(redisValue);

            //if (!value.SequenceEqual(value2!)) throw new InvalidOperationException();
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

    public RedisValue Serialize(in T?[]? value) => value == null ? RedisValues.Zero : UnmanagedEnumerableNullableFormatter.SerializeArray(value);

    public RedisValue Serialize(in T[]? value) => value == null ? RedisValues.Zero : UnmanagedEnumerableFormatter.SerializeArray(value);
}