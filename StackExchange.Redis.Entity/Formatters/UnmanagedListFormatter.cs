using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public delegate TList NewList<TList, T>(int capacity) where TList : IList<T>;

public class UnmanagedListFormatter<TList, T> : IRedisValueFormatter<TList>
    where TList : IList<T>
    where T : unmanaged
{
    private readonly NewList<TList, T> _newList;
    private readonly TList _empty;

    public UnmanagedListFormatter(NewList<TList, T> newList)
    {
        _newList = newList ?? throw new ArgumentNullException(nameof(newList));
        _empty = newList(0);
    }

    public void Deserialize(in RedisValue redisValue, ref TList? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = default;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = _empty;
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T>();
            var length = span.Length / size;
            ref byte bytes = ref MemoryMarshal.GetReference(span);

            if (value == null) value = _newList(length);
            else if (value.Count > 0) value.Clear();

            for (int i = 0, b = 0; i < length; i++, b += size)
            {
                value.Add(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b)));
            }
        }
    }

    public RedisValue Serialize(in TList? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }
}