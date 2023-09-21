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
            spanRef = ref Unsafe.Add(ref spanRef, span.Length);

            for (int i = 0, b = size; i < count; i++, b += size)
            {
                stack.Push(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, -b)));
            }
        }
        else
        {
            throw new NotImplementedException($"{buffer.GetType().FullName} not implemented add");
        }
    }

    internal static bool Deserialize<T>(ref IEnumerable<T> value, in ReadOnlySpan<byte> span, int size, int length) where T : unmanaged
    {
        ref byte spanRef = ref MemoryMarshal.GetReference(span);

        if (value is T[] array)
        {
            if (array.Length != length)
            {
                array = new T[length];
                value = array;
            }

            for (int i = 0, b = 0; i < array.Length; i++, b += size)
            {
                array[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));
            }
        }
        else if (value is ICollection<T> collection)
        {
            if (collection.IsReadOnly) return false;
            else if (value is IList<T> ilist)
            {
                var count = ilist.Count;
                if (count < length)
                {
                    if (ilist is List<T> list && list.Capacity < length) list.Capacity = length;

                    var b = 0;

                    if (count > 0)
                    {
                        for (int i = 0; i < ilist.Count; i++, b += size)
                        {
                            ilist[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));
                        }

                        length -= count;
                    }

                    for (int i = 0; i < length; i++, b += size)
                    {
                        ilist.Add(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)));
                    }
                }
                else
                {
                    for (int i = 0, b = 0; i < length; i++, b += size)
                    {
                        ilist[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));
                    }

                    for (int i = count - 1; i >= length; i--) ilist.RemoveAt(i);
                }
            }
            else
            {
                if (collection.Count > 0) collection.Clear();

                for (int i = 0, b = 0; i < length; i++, b += size)
                {
                    collection.Add(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)));
                }
            }
        }
        else if (value is Queue<T> queue)
        {
            if (queue.Count > 0) queue.Clear();

#if NET6_0_OR_GREATER
            queue.EnsureCapacity(length);
#endif
            for (int i = 0, b = 0; i < length; i++, b += size)
            {
                queue.Enqueue(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)));
            }
        }
        else if (value is Stack<T> stack)
        {
            if (stack.Count > 0) stack.Clear();

#if NET6_0_OR_GREATER
            stack.EnsureCapacity(length);
#endif
            spanRef = ref Unsafe.Add(ref spanRef, span.Length);

            for (int i = 0, b = size; i < length; i++, b += size)
            {
                stack.Push(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, -b)));
            }
        }
        else
        {
            throw new NotImplementedException($"{value.GetType().FullName} not implemented add");
        }

        return true;
    }

    internal static RedisValue Serialize<T>(in IEnumerable<T>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value is T[] array) return SerializeArray(array);
        if (value is IReadOnlyList<T> readOnlyList) return SerializeReadOnlyList(readOnlyList);
        if (value is IList<T> list) return SerializeList(list);
        if (value is IReadOnlyCollection<T> readOnlyCollection) return SerializeReadOnlyCollection(readOnlyCollection);
        if (value is ICollection<T> collection) return SerializeCollection(collection);
#if NET6_0_OR_GREATER
        if (value.TryGetNonEnumeratedCount(out var count))
        {
#else
        if (value is System.Collections.ICollection collectionNonGeneric)
        {
            var count = collectionNonGeneric.Count;
#endif
            if (count == 0) return RedisValue.EmptyString;

            var size = Unsafe.SizeOf<T>();
            var maxLength = Util.RedisValueMaxLength / size;
            if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

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

    internal static RedisValue SerializeArray<T>(in T[] value)
    {
        var length = value.Length;
        if (length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = Util.RedisValueMaxLength / size;
        if (length > maxLength) throw Ex.InvalidLengthCollection(typeof(T), length, maxLength);

        var bytes = new byte[size * length];

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyList<T>(in IReadOnlyList<T> value)
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = Util.RedisValueMaxLength / size;
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var bytes = new byte[size * count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeList<T>(in IList<T> value)
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = Util.RedisValueMaxLength / size;
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var bytes = new byte[size * count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyCollection<T>(in IReadOnlyCollection<T> value)
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = Util.RedisValueMaxLength / size;
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var bytes = new byte[size * count];
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
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = Util.RedisValueMaxLength / size;
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var bytes = new byte[size * count];
        int b = 0;

        foreach (var item in value)
        {
            Unsafe.WriteUnaligned(ref bytes[b], item);
            b += size;
        }

        return bytes;
    }
}