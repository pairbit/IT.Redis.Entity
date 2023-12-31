﻿using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace IT.Redis.Entity.Internal;

internal class StringEnumerableFormatter
{
    private const int Size = 4;
    internal const int MinLength = Size * 2;

    internal static void Build(Collections.Factory.TryAdd<string?> add, in BuildState state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;

        ref byte spanRef = ref MemoryMarshal.GetReference(span);
        span = span.Slice(Size * length + Size);

        for (int i = 0, b = Size; i < length; i++, b += Size)
        {
            add(UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
        }
    }

    internal static void BuildReverse(Collections.Factory.TryAdd<string?> add, in BuildState state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;

        ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

        for (int i = 0, b = Size; i < length; i++, b += Size)
        {
            add(UnsafeReader.ReadStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
        }
    }

    internal static bool Deserialize(ref IEnumerable<string?> value, in BuildState state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;

        if (value is string?[] array)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            if (array.Length != length)
            {
                array = new string?[length];
                value = array;
            }

            for (int i = 0, b = Size; i < array.Length; i++, b += Size)
            {
                array[i] = UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding);
            }
        }
        else if (value is ICollection<string?> collection)
        {
            if (collection.IsReadOnly) return false;

            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            if (value is IList<string?> ilist)
            {
                var count = ilist.Count;
                if (count < length)
                {
                    if (ilist is List<string?> list && list.Capacity < length) list.Capacity = length;

                    var b = Size;

                    if (count > 0)
                    {
                        for (int i = 0; i < ilist.Count; i++, b += Size)
                        {
                            ilist[i] = UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding);
                        }

                        length -= count;
                    }

                    for (int i = 0; i < length; i++, b += Size)
                    {
                        ilist.Add(UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
                    }
                }
                else
                {
                    for (int i = 0, b = Size; i < length; i++, b += Size)
                    {
                        ilist[i] = UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding);
                    }

                    for (int i = count - 1; i >= length; i--) ilist.RemoveAt(i);
                }
            }
            else
            {
                if (collection.Count > 0) collection.Clear();

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                if (collection is HashSet<string?> hashSet) hashSet.EnsureCapacity(length);
#endif
                for (int i = 0, b = Size; i < length; i++, b += Size)
                {
                    collection.Add(UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
                }
            }
        }
        else if (value is Queue<string?> queue)
        {
            if (queue.Count > 0) queue.Clear();

#if NET6_0_OR_GREATER
            queue.EnsureCapacity(length);
#endif
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                queue.Enqueue(UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else if (value is Stack<string?> stack)
        {
            if (stack.Count > 0) stack.Clear();

#if NET6_0_OR_GREATER
            stack.EnsureCapacity(length);
#endif
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                stack.Push(UnsafeReader.ReadStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (value is ConcurrentStack<string?> cStack)
        {
            cStack.Clear();

            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                cStack.Push(UnsafeReader.ReadStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (value is ConcurrentBag<string?> bag)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            bag.Clear();
#else
            if (!bag.IsEmpty) throw Ex.ClearNotSupported(bag.GetType());
#endif
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                bag.Add(UnsafeReader.ReadStringFromEnd(ref Unsafe.Add(ref spanRef, -b), ref span, encoding));
            }
        }
        else if (value is IProducerConsumerCollection<string?> pcCollection)
        {
            if (pcCollection is ConcurrentQueue<string?> cQueue)
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
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                pcCollection.AddOrThrow(UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else if (value is BlockingCollection<string?> bCollection)
        {
            if (bCollection.Count > 0) throw Ex.ClearNotSupported(bCollection.GetType());

            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                bCollection.Add(UnsafeReader.ReadString(ref Unsafe.Add(ref spanRef, b), ref span, encoding));
            }
        }
        else
        {
            throw new NotImplementedException($"{value.GetType().FullName} not implemented add");
        }

        return true;
    }

    internal static RedisValue Serialize(Encoding encoding, in IEnumerable<string?>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value is string?[] array) return SerializeArray(encoding, array);
        if (value is IReadOnlyList<string?> readOnlyList) return SerializeReadOnlyList(encoding, readOnlyList);
        if (value is IList<string?> list) return SerializeList(encoding, list);
        if (value is IReadOnlyCollection<string?> readOnlyCollection) return SerializeReadOnlyCollection(encoding, readOnlyCollection);
        if (value is ICollection<string?> collection) return SerializeCollection(encoding, collection);
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
            foreach (var str in value)
            {
                length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
            }

            if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

            var bytes = new byte[length];

            Unsafe.WriteUnaligned(ref bytes[0], count);

            var span = bytes.AsSpan(Size * count + Size);
            var b = Size;

            foreach (var str in value)
            {
                if (str == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
                else if (str.Length > 0)
                {
                    var written = encoding.GetBytes(str, span);
                    Unsafe.WriteUnaligned(ref bytes[b], written);
                    span = span.Slice(written);
                }
                b += Size;
            }

            return bytes;
        }
        return SerializeArray(encoding, value.ToArray());
    }

    internal static RedisValue SerializeArray(Encoding encoding, in string?[] value)
    {
        if (value.Length == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Length; i++)
        {
            var str = value[i];
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], value.Length);

        var span = bytes.AsSpan(Size * value.Length + Size);

        for (int i = 0, b = Size; i < value.Length; i++, b += Size)
        {
            var str = value[i];
            if (str == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (str.Length > 0)
            {
                var written = encoding.GetBytes(str, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyList(Encoding encoding, in IReadOnlyList<string?> value)
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Count; i++)
        {
            var str = value[i];
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], count);

        var span = bytes.AsSpan(Size * count + Size);

        for (int i = 0, b = Size; i < value.Count; i++, b += Size)
        {
            var str = value[i];
            if (str == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (str.Length > 0)
            {
                var written = encoding.GetBytes(str, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
        }

        return bytes;
    }

    private static RedisValue SerializeList(Encoding encoding, in IList<string?> value)
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Count; i++)
        {
            var str = value[i];
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], count);

        var span = bytes.AsSpan(Size * count + Size);

        for (int i = 0, b = Size; i < value.Count; i++, b += Size)
        {
            var str = value[i];
            if (str == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (str.Length > 0)
            {
                var written = encoding.GetBytes(str, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyCollection(Encoding encoding, in IReadOnlyCollection<string?> value)
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var length = Size;
        foreach (var str in value)
        {
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], count);

        var span = bytes.AsSpan(Size * count + Size);
        var b = Size;

        foreach (var str in value)
        {
            if (str == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (str.Length > 0)
            {
                var written = encoding.GetBytes(str, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
            b += Size;
        }

        return bytes;
    }

    private static RedisValue SerializeCollection(Encoding encoding, in ICollection<string?> value)
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var length = Size;
        foreach (var str in value)
        {
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], count);

        var span = bytes.AsSpan(Size * count + Size);
        var b = Size;

        foreach (var str in value)
        {
            if (str == null) Unsafe.WriteUnaligned(ref bytes[b], int.MaxValue);
            else if (str.Length > 0)
            {
                var written = encoding.GetBytes(str, span);
                Unsafe.WriteUnaligned(ref bytes[b], written);
                span = span.Slice(written);
            }
            b += Size;
        }

        return bytes;
    }
}