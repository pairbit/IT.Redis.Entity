using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace StackExchange.Redis.Entity.Internal;

internal class StringEnumerableFormatter
{
    private const int Size = 4;
    internal const int MinLength = Size * 2;

    internal static void Build(IEnumerable<string?> buffer, in BuildState state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;

        if (buffer is string?[] array)
        {
            //if (array.Length < count) throw new InvalidOperationException();

            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < array.Length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == 0) array[i] = string.Empty;
                else if (strlen != int.MaxValue)
                {
                    array[i] = encoding.GetString(span.Slice(0, strlen));
                    span = span.Slice(strlen);
                }
            }
        }
        else if (buffer is ICollection<string?> collection)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) collection.Add(null);
                else if (strlen == 0) collection.Add(string.Empty);
                else
                {
                    collection.Add(encoding.GetString(span.Slice(0, strlen)));
                    span = span.Slice(strlen);
                }
            }
        }
        else if (buffer is Queue<string?> queue)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) queue.Enqueue(null);
                else if (strlen == 0) queue.Enqueue(string.Empty);
                else
                {
                    queue.Enqueue(encoding.GetString(span.Slice(0, strlen)));
                    span = span.Slice(strlen);
                }
            }
        }
        else if (buffer is Stack<string?> stack)
        {
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, -b));

                if (strlen == int.MaxValue) stack.Push(null);
                else if (strlen == 0) stack.Push(string.Empty);
                else
                {
                    strlen = span.Length - strlen;
                    stack.Push(encoding.GetString(span.Slice(strlen)));
                    span = span.Slice(0, strlen);
                }
            }
        }
        else if (buffer is ConcurrentStack<string?> cStack)
        {
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, -b));

                if (strlen == int.MaxValue) cStack.Push(null);
                else if (strlen == 0) cStack.Push(string.Empty);
                else
                {
                    strlen = span.Length - strlen;
                    cStack.Push(encoding.GetString(span.Slice(strlen)));
                    span = span.Slice(0, strlen);
                }
            }
        }
        else if (buffer is ConcurrentBag<string?> bag)
        {
            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, -b));

                if (strlen == int.MaxValue) bag.Add(null);
                else if (strlen == 0) bag.Add(string.Empty);
                else
                {
                    strlen = span.Length - strlen;
                    bag.Add(encoding.GetString(span.Slice(strlen)));
                    span = span.Slice(0, strlen);
                }
            }
        }
        else if (buffer is IProducerConsumerCollection<string?> pcCollection)
        {
            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) pcCollection.AddOrThrow(null);
                else if (strlen == 0) pcCollection.AddOrThrow(string.Empty);
                else
                {
                    pcCollection.AddOrThrow(encoding.GetString(span.Slice(0, strlen)));
                    span = span.Slice(strlen);
                }
            }
        }
        else
        {
            throw new NotImplementedException($"{buffer.GetType().FullName} not implemented add");
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
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) array[i] = null;
                else if (strlen == 0) array[i] = string.Empty;
                else
                {
                    array[i] = encoding.GetString(span.Slice(0, strlen));
                    span = span.Slice(strlen);
                }
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
                            var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                            if (strlen == int.MaxValue) ilist[i] = null;
                            else if (strlen == 0) ilist[i] = string.Empty;
                            else
                            {
                                ilist[i] = encoding.GetString(span.Slice(0, strlen));
                                span = span.Slice(strlen);
                            }
                        }

                        length -= count;
                    }

                    for (int i = 0; i < length; i++, b += Size)
                    {
                        var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                        if (strlen == int.MaxValue) ilist.Add(null);
                        else if (strlen == 0) ilist.Add(string.Empty);
                        else
                        {
                            ilist.Add(encoding.GetString(span.Slice(0, strlen)));
                            span = span.Slice(strlen);
                        }
                    }
                }
                else
                {
                    for (int i = 0, b = Size; i < length; i++, b += Size)
                    {
                        var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                        if (strlen == int.MaxValue) ilist[i] = null;
                        else if (strlen == 0) ilist[i] = string.Empty;
                        else
                        {
                            ilist[i] = encoding.GetString(span.Slice(0, strlen));
                            span = span.Slice(strlen);
                        }
                    }

                    for (int i = count - 1; i >= length; i--) ilist.RemoveAt(i);
                }
            }
            else
            {
                if (collection.Count > 0) collection.Clear();

                for (int i = 0, b = Size; i < length; i++, b += Size)
                {
                    var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                    if (strlen == int.MaxValue) collection.Add(null);
                    else if (strlen == 0) collection.Add(string.Empty);
                    else
                    {
                        collection.Add(encoding.GetString(span.Slice(0, strlen)));
                        span = span.Slice(strlen);
                    }
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
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) queue.Enqueue(null);
                else if (strlen == 0) queue.Enqueue(string.Empty);
                else
                {
                    queue.Enqueue(encoding.GetString(span.Slice(0, strlen)));
                    span = span.Slice(strlen);
                }
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
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, -b));

                if (strlen == int.MaxValue) stack.Push(null);
                else if (strlen == 0) stack.Push(string.Empty);
                else
                {
                    strlen = span.Length - strlen;
                    stack.Push(encoding.GetString(span.Slice(strlen)));
                    span = span.Slice(0, strlen);
                }
            }
        }
        else if (value is ConcurrentStack<string?> cStack)
        {
            cStack.Clear();

            ref byte spanRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, -b));

                if (strlen == int.MaxValue) cStack.Push(null);
                else if (strlen == 0) cStack.Push(string.Empty);
                else
                {
                    strlen = span.Length - strlen;
                    cStack.Push(encoding.GetString(span.Slice(strlen)));
                    span = span.Slice(0, strlen);
                }
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
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, -b));

                if (strlen == int.MaxValue) bag.Add(null);
                else if (strlen == 0) bag.Add(string.Empty);
                else
                {
                    strlen = span.Length - strlen;
                    bag.Add(encoding.GetString(span.Slice(strlen)));
                    span = span.Slice(0, strlen);
                }
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
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) pcCollection.AddOrThrow(null);
                else if (strlen == 0) pcCollection.AddOrThrow(string.Empty);
                else
                {
                    pcCollection.AddOrThrow(encoding.GetString(span.Slice(0, strlen)));
                    span = span.Slice(strlen);
                }
            }
        }
        else if (value is BlockingCollection<string?> bCollection)
        {
            if (bCollection.Count > 0) throw Ex.ClearNotSupported(bCollection.GetType());

            ref byte spanRef = ref MemoryMarshal.GetReference(span);
            span = span.Slice(Size * length + Size);

            for (int i = 0, b = Size; i < length; i++, b += Size)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, b));

                if (strlen == int.MaxValue) bCollection.Add(null);
                else if (strlen == 0) bCollection.Add(string.Empty);
                else
                {
                    bCollection.Add(encoding.GetString(span.Slice(0, strlen)));
                    span = span.Slice(strlen);
                }
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
        if (value.Count == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Count; i++)
        {
            var str = value[i];
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], value.Count);

        var span = bytes.AsSpan(Size * value.Count + Size);

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
        if (value.Count == 0) return RedisValue.EmptyString;

        var length = Size;
        for (int i = 0; i < value.Count; i++)
        {
            var str = value[i];
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], value.Count);

        var span = bytes.AsSpan(Size * value.Count + Size);

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
        if (value.Count == 0) return RedisValue.EmptyString;

        var length = Size;
        foreach (var str in value)
        {
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], value.Count);

        var span = bytes.AsSpan(Size * value.Count + Size);
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
        if (value.Count == 0) return RedisValue.EmptyString;

        var length = Size;
        foreach (var str in value)
        {
            length += Size + (str == null || str.Length == 0 ? 0 : encoding.GetByteCount(str.AsSpan()));
        }

        if (length > Util.RedisValueMaxLength) throw Ex.InvalidLengthCollection(typeof(string?[]), length, Util.RedisValueMaxLength);

        var bytes = new byte[length];

        Unsafe.WriteUnaligned(ref bytes[0], value.Count);

        var span = bytes.AsSpan(Size * value.Count + Size);
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