using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace StackExchange.Redis.Entity.Internal;

public static class StringDictionaryFormatter
{
    private const int Size = 4;
    internal const int DoubleSize = Size * 2;
    internal const int MinLength = Size * 3;

    internal static void Build(IEnumerable<KeyValuePair<string?, string?>> buffer, in BuildState state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;

        if (buffer is KeyValuePair<string?, string?>[] array)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < array.Length; i++, b += DoubleSize)
            {
                array[i] = UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding);
            }
        }
        else if (buffer is ICollection<KeyValuePair<string?, string?>> collection)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                collection.Add(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else if (buffer is Queue<KeyValuePair<string?, string?>> queue)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                queue.Enqueue(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else if (buffer is Stack<KeyValuePair<string?, string?>> stack)
        {
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                stack.Push(UnsafeReader.ReadPairStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (buffer is ConcurrentStack<KeyValuePair<string?, string?>> cStack)
        {
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                cStack.Push(UnsafeReader.ReadPairStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (buffer is ConcurrentBag<KeyValuePair<string?, string?>> bag)
        {
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                bag.Add(UnsafeReader.ReadPairStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (buffer is IProducerConsumerCollection<KeyValuePair<string?, string?>> pcCollection)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                pcCollection.AddOrThrow(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else
        {
            throw new NotImplementedException($"{buffer.GetType().FullName} not implemented add");
        }
    }

    internal static bool Deserialize(ref IEnumerable<KeyValuePair<string?, string?>> value, in BuildState state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;

        if (value is KeyValuePair<string?, string?>[] array)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            if (array.Length != length)
            {
                array = new KeyValuePair<string?, string?>[length];
                value = array;
            }

            for (int i = 0, b = Size; i < array.Length; i++, b += DoubleSize)
            {
                array[i] = UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding);
            }
        }
        else if (value is ICollection<KeyValuePair<string?, string?>> collection)
        {
            if (collection.IsReadOnly) return false;

            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            if (value is IList<KeyValuePair<string?, string?>> ilist)
            {
                var count = ilist.Count;
                if (count < length)
                {
                    if (ilist is List<KeyValuePair<string?, string?>> list && list.Capacity < length) list.Capacity = length;

                    var b = Size;

                    if (count > 0)
                    {
                        for (int i = 0; i < ilist.Count; i++, b += DoubleSize)
                        {
                            ilist[i] = UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding);
                        }

                        length -= count;
                    }

                    for (int i = 0; i < length; i++, b += DoubleSize)
                    {
                        ilist.Add(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
                    }
                }
                else
                {
                    for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
                    {
                        ilist[i] = UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding);
                    }

                    for (int i = count - 1; i >= length; i--) ilist.RemoveAt(i);
                }
            }
            else
            {
                if (collection.Count > 0) collection.Clear();

                for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
                {
                    collection.Add(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
                }
            }
        }
        else if (value is Queue<KeyValuePair<string?, string?>> queue)
        {
            if (queue.Count > 0) queue.Clear();

#if NET6_0_OR_GREATER
            queue.EnsureCapacity(length);
#endif
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                queue.Enqueue(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else if (value is Stack<KeyValuePair<string?, string?>> stack)
        {
            if (stack.Count > 0) stack.Clear();

#if NET6_0_OR_GREATER
            stack.EnsureCapacity(length);
#endif
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                stack.Push(UnsafeReader.ReadPairStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (value is ConcurrentStack<KeyValuePair<string?, string?>> cStack)
        {
            cStack.Clear();

            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                cStack.Push(UnsafeReader.ReadPairStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (value is ConcurrentBag<KeyValuePair<string?, string?>> bag)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            bag.Clear();
#else
            if (!bag.IsEmpty) throw Ex.ClearNotSupported(bag.GetType());
#endif
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                bag.Add(UnsafeReader.ReadPairStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (value is IProducerConsumerCollection<KeyValuePair<string?, string?>> pcCollection)
        {
            if (pcCollection is ConcurrentQueue<KeyValuePair<string?, string?>> cQueue)
            {
                if (!cQueue.IsEmpty)
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                    cQueue.Clear();
#else
                    throw Ex.ClearNotSupported(pcCollection.GetType());
#endif
            }
            else if (pcCollection.Count > 0) throw Ex.ClearNotSupported(pcCollection.GetType());

            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                pcCollection.AddOrThrow(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else if (value is BlockingCollection<KeyValuePair<string?, string?>> bCollection)
        {
            if (bCollection.Count > 0) throw Ex.ClearNotSupported(bCollection.GetType());

            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(DoubleSize * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += DoubleSize)
            {
                bCollection.Add(UnsafeReader.ReadPairString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else
        {
            throw new NotImplementedException($"{value.GetType().FullName} not implemented add");
        }

        return true;
    }

    internal static RedisValue Serialize(Encoding encoding, in IEnumerable<KeyValuePair<string?, string?>>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value is KeyValuePair<string?, string?>[] array) return SerializeArray(encoding, array);
        if (value is IReadOnlyList<KeyValuePair<string?, string?>> readOnlyList) return SerializeReadOnlyList(encoding, readOnlyList);
        if (value is IList<KeyValuePair<string?, string?>> list) return SerializeList(encoding, list);
        if (value is IReadOnlyCollection<KeyValuePair<string?, string?>> readOnlyCollection) return SerializeReadOnlyCollection(encoding, readOnlyCollection);
        if (value is ICollection<KeyValuePair<string?, string?>> collection) return SerializeCollection(encoding, collection);
#if NET6_0_OR_GREATER
        if (value.TryGetNonEnumeratedCount(out var count))
        {
#else
        if (value is System.Collections.ICollection collectionNonGeneric)
        {
            var count = collectionNonGeneric.Count;
#endif
            if (count == 0) return RedisValue.EmptyString;

            var length = Size;
            foreach (var pair in value)
            {
                var key = pair.Key;
                var val = pair.Value;
                length += Size + (key == null || key.Length == 0 ? 0 : encoding.GetByteCount(key.AsSpan())) +
                          Size + (val == null || val.Length == 0 ? 0 : encoding.GetByteCount(val.AsSpan()));
            }

            if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(value.GetType(), length, Util.RedisValueMaxLength);

            var bytes = new byte[length];

            Unsafe.WriteUnaligned(ref bytes[0], count);

            var span = bytes.AsSpan(2 * Size * count + Size);
            var b = Size;

            foreach (var pair in value)
            {
                var key = pair.Key;
                if (key == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
                else if (key.Length > 0)
                {
                    var written = encoding.GetBytes(key, span);
                    Unsafe.WriteUnaligned(ref bytes[b], written);
                    span = span.Slice(written);
                }
                b += Size;

                var val = pair.Value;
                if (val == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
                else if (val.Length > 0)
                {
                    var written = encoding.GetBytes(val, span);
                    Unsafe.WriteUnaligned(ref bytes[b], written);
                    span = span.Slice(written);
                }
                b += Size;
            }

            return bytes;
        }
        return SerializeArray(encoding, value.ToArray());
    }

    internal static RedisValue SerializeArray(Encoding encoding, in KeyValuePair<string?, string?>[] value)
    {
        if (value.Length == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Length; i++)
        {
            var pair = value[i];
            var key = pair.Key;
            var val = pair.Value;
            length += Size + (key == null || key.Length == 0 ? 0 : encoding.GetByteCount(key.AsSpan())) +
                      Size + (val == null || val.Length == 0 ? 0 : encoding.GetByteCount(val.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(value.GetType(), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], value.Length);

        var span = bytes.AsSpan(2 * Size * value.Length + Size);

        for (int i = 0, b = Size; i < value.Length; i++, b += Size)
        {
            var pair = value[i];
            var key = pair.Key;
            if (key == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (key.Length > 0)
            {
                var written = encoding.GetBytes(key, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
            b += Size;

            var val = pair.Value;
            if (val == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (val.Length > 0)
            {
                var written = encoding.GetBytes(val, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyList(Encoding encoding, in IReadOnlyList<KeyValuePair<string?, string?>> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Count; i++)
        {
            var pair = value[i];
            var key = pair.Key;
            var val = pair.Value;
            length += Size + (key == null || key.Length == 0 ? 0 : encoding.GetByteCount(key.AsSpan())) +
                      Size + (val == null || val.Length == 0 ? 0 : encoding.GetByteCount(val.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(value.GetType(), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], value.Count);

        var span = bytes.AsSpan(2 * Size * value.Count + Size);

        for (int i = 0, b = Size; i < value.Count; i++, b += Size)
        {
            var pair = value[i];
            var key = pair.Key;
            if (key == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (key.Length > 0)
            {
                var written = encoding.GetBytes(key, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
            b += Size;

            var val = pair.Value;
            if (val == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (val.Length > 0)
            {
                var written = encoding.GetBytes(val, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
        }

        return bytes;
    }

    private static RedisValue SerializeList(Encoding encoding, in IList<KeyValuePair<string?, string?>> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        throw new NotImplementedException();
    }

    private static RedisValue SerializeReadOnlyCollection(Encoding encoding, in IReadOnlyCollection<KeyValuePair<string?, string?>> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        throw new NotImplementedException();
    }

    private static RedisValue SerializeCollection(Encoding encoding, in ICollection<KeyValuePair<string?, string?>> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        throw new NotImplementedException();
    }
}