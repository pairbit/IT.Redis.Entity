using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Internal;

internal static class UnmanagedEnumerableFormatter
{
    internal static void Build<T>(IEnumerable<T> buffer, in ReadOnlyMemory<byte> memory)
    {
        var span = memory.Span;
        var size = Unsafe.SizeOf<T>();
        var count = span.Length / size;
        ref byte spanRef = ref MemoryMarshal.GetReference(span);

        if (buffer is T[] array)
        {
            //if (array.Length < count) throw new InvalidOperationException();

            for (int i = 0, b = 0; i < array.Length; i++, b += size)
            {
                array[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));
            }
        }
        else if (buffer is ICollection<T> collection)
        {
            for (int i = 0, b = 0; i < count; i++, b += size)
            {
                collection.Add(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)));
            }
        }
        else if (buffer is Queue<T> queue)
        {
            for (int i = 0, b = 0; i < count; i++, b += size)
            {
                queue.Enqueue(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)));
            }
        }
        else if (buffer is Stack<T> stack)
        {
            for (int i = 0, b = 0; i < count; i++, b += size)
            {
                stack.Push(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)));
            }
        }
        else
        {
            throw new NotImplementedException($"{buffer.GetType().FullName} not implemented add");
        }
    }

    internal static RedisValue Serialize<T>(in IEnumerable<T>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value is T[] array) return SerializeArray(array);
        if (value is IReadOnlyList<T> readOnlyList) return SerializeReadOnlyList(readOnlyList);
        if (value is IList<T> list) return SerializeList(list);
        if (value is IReadOnlyCollection<T> readOnlyCollection) return SerializeReadOnlyCollection(readOnlyCollection);
        if (value is ICollection<T> collection) return SerializeCollection(collection);
        if (value.TryGetNonEnumeratedCount(out var count))
        {
            if (count == 0) return RedisValue.EmptyString;

            var size = Unsafe.SizeOf<T>();
            var bytes = new byte[size * count];
            var b = 0;
            foreach (var item in value)
            {
                Unsafe.WriteUnaligned(ref bytes[b], item);
                b += size;
            }
            return bytes;
        }
        return SerializeArray(value.ToArray());
    }

    private static RedisValue SerializeArray<T>(in T[] value)
    {
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Length];

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyList<T>(in IReadOnlyList<T> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeList<T>(in IList<T> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyCollection<T>(in IReadOnlyCollection<T> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Count];
        int b = 0;

        foreach (var item in value)
        {
            Unsafe.WriteUnaligned(ref bytes[b], item);
            b += size;
        }

        return bytes;
    }

    private static RedisValue SerializeCollection<T>(in ICollection<T> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Count];
        int b = 0;

        foreach (var item in value)
        {
            Unsafe.WriteUnaligned(ref bytes[b], item);
            b += size;
        }

        return bytes;
    }
}