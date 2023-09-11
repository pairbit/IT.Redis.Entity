using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public class UnmanagedICollectionFormatter<T> : IRedisValueFormatter<ICollection<T>> where T : unmanaged
{
    public void Deserialize(in RedisValue redisValue, ref ICollection<T>? value)
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
            var count = span.Length / size;
            ref byte bytes = ref MemoryMarshal.GetReference(span);

            if (value == null || value.IsReadOnly)
            {
                var array = value as T[];

                if (array == null || array.Length != count)
                    array = new T[count];

                for (int i = 0, b = 0; i < array.Length; i++, b += size)
                {
                    array[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b));
                }

                value = array;
            }
            else
            {
                if (value.Count > 0) value.Clear();

                for (int i = 0, b = 0; i < count; i++, b += size)
                {
                    value.Add(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b)));
                }
            }
        }
    }

    public RedisValue Serialize(in ICollection<T>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Count];
        int i = 0;

        foreach (var item in value)
        {
            Unsafe.WriteUnaligned(ref bytes[i], item);
            i += size;
        }

        return bytes;
    }
}