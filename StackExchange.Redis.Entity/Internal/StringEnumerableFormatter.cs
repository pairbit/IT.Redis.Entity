using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace StackExchange.Redis.Entity.Internal;

internal class StringEnumerableFormatter
{
    private const int Size = 4;
    internal const int MinLength = Size * 2;

    public readonly struct State
    {
        public readonly ReadOnlyMemory<byte> Memory;
        public readonly Encoding Encoding;
        public readonly int Length;

        public State(ReadOnlyMemory<byte> memory, Encoding encoding, int length)
        {
            Memory = memory;
            Encoding = encoding;
            Length = length;
        }
    }

    internal static void Build(IEnumerable<string?> buffer, in State state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;
        ref byte spanRef = ref MemoryMarshal.GetReference(span);
        var offset = Size;

        if (buffer is string?[] array)
        {
            //if (array.Length < count) throw new InvalidOperationException();

            for (int i = 0; i < array.Length; i++)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                if (strlen == int.MaxValue)
                {
                    array[i] = null;
                    offset += Size;
                }
                else if (strlen == 0)
                {
                    array[i] = string.Empty;
                    offset += Size;
                }
                else
                {
                    array[i] = encoding.GetString(span.Slice(offset + Size, strlen));
                    offset += Size + strlen;
                }
            }
        }
        else if (buffer is ICollection<string?> collection)
        {
            for (int i = 0; i < length; i++)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                if (strlen == int.MaxValue)
                {
                    collection.Add(null);
                    offset += Size;
                }
                else if (strlen == 0)
                {
                    collection.Add(string.Empty);
                    offset += Size;
                }
                else
                {
                    collection.Add(encoding.GetString(span.Slice(offset + Size, strlen)));
                    offset += Size + strlen;
                }
            }
        }
        else if (buffer is Queue<string?> queue)
        {
            for (int i = 0; i < length; i++)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                if (strlen == int.MaxValue)
                {
                    queue.Enqueue(null);
                    offset += Size;
                }
                else if (strlen == 0)
                {
                    queue.Enqueue(string.Empty);
                    offset += Size;
                }
                else
                {
                    queue.Enqueue(encoding.GetString(span.Slice(offset + Size, strlen)));
                    offset += Size + strlen;
                }
            }
        }
        else if (buffer is Stack<string?> stack)
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException($"{buffer.GetType().FullName} not implemented add");
        }
    }

    internal static bool Deserialize(ref IEnumerable<string?> value, in State state)
    {
        var length = state.Length;
        var encoding = state.Encoding;
        var span = state.Memory.Span;
        ref byte spanRef = ref MemoryMarshal.GetReference(span);
        var offset = Size;

        if (value is string?[] array)
        {
            if (array.Length != length)
            {
                array = new string?[length];
                value = array;
            }

            for (int i = 0; i < array.Length; i++)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                if (strlen == int.MaxValue)
                {
                    array[i] = null;
                    offset += Size;
                }
                else if (strlen == 0)
                {
                    array[i] = string.Empty;
                    offset += Size;
                }
                else
                {
                    array[i] = encoding.GetString(span.Slice(offset + Size, strlen));
                    offset += Size + strlen;
                }
            }
        }
        else if (value is ICollection<string?> collection)
        {
            if (collection.IsReadOnly) return false;
            else if (value is IList<string?> ilist)
            {
                var count = ilist.Count;
                if (count < length)
                {
                    if (ilist is List<string?> list && list.Capacity < length) list.Capacity = length;

                    if (count > 0)
                    {
                        for (int i = 0; i < ilist.Count; i++)
                        {
                            var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                            if (strlen == int.MaxValue)
                            {
                                ilist[i] = null;
                                offset += Size;
                            }
                            else if (strlen == 0)
                            {
                                ilist[i] = string.Empty;
                                offset += Size;
                            }
                            else
                            {
                                ilist[i] = encoding.GetString(span.Slice(offset + Size, strlen));
                                offset += Size + strlen;
                            }
                        }

                        length -= count;
                    }

                    for (int i = 0; i < length; i++)
                    {
                        var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                        if (strlen == int.MaxValue)
                        {
                            ilist.Add(null);
                            offset += Size;
                        }
                        else if (strlen == 0)
                        {
                            ilist.Add(string.Empty);
                            offset += Size;
                        }
                        else
                        {
                            ilist.Add(encoding.GetString(span.Slice(offset + Size, strlen)));
                            offset += Size + strlen;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                        if (strlen == int.MaxValue)
                        {
                            ilist[i] = null;
                            offset += Size;
                        }
                        else if (strlen == 0)
                        {
                            ilist[i] = string.Empty;
                            offset += Size;
                        }
                        else
                        {
                            ilist[i] = encoding.GetString(span.Slice(offset + Size, strlen));
                            offset += Size + strlen;
                        }
                    }

                    for (int i = count - 1; i >= length; i--) ilist.RemoveAt(i);
                }
            }
            else
            {
                if (collection.Count > 0) collection.Clear();

                for (int i = 0; i < length; i++)
                {
                    var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                    if (strlen == int.MaxValue)
                    {
                        collection.Add(null);
                        offset += Size;
                    }
                    else if (strlen == 0)
                    {
                        collection.Add(string.Empty);
                        offset += Size;
                    }
                    else
                    {
                        collection.Add(encoding.GetString(span.Slice(offset + Size, strlen)));
                        offset += Size + strlen;
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
            for (int i = 0; i < length; i++)
            {
                var strlen = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, offset));

                if (strlen == int.MaxValue)
                {
                    queue.Enqueue(null);
                    offset += Size;
                }
                else if (strlen == 0)
                {
                    queue.Enqueue(string.Empty);
                    offset += Size;
                }
                else
                {
                    queue.Enqueue(encoding.GetString(span.Slice(offset + Size, strlen)));
                    offset += Size + strlen;
                }
            }
        }
        else if (value is Stack<string?> stack)
        {
            if (stack.Count > 0) stack.Clear();

#if NET6_0_OR_GREATER
            stack.EnsureCapacity(length);
#endif
            throw new NotImplementedException();
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

            var offset = Size;
            var span = bytes.AsSpan();

            foreach (var str in value)
            {
                if (str == null)
                {
                    Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                    offset += Size;
                }
                else if (str.Length == 0)
                {
                    Unsafe.WriteUnaligned(ref bytes[offset], 0);

                    offset += Size;
                }
                else
                {
                    var written = encoding.GetBytes(str, span.Slice(offset + Size));

                    Unsafe.WriteUnaligned(ref bytes[offset], written);

                    offset += Size + written;
                }
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

        var offset = Size;
        var span = bytes.AsSpan();

        for (int i = 0; i < value.Length; i++)
        {
            var str = value[i];

            if (str == null)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                offset += Size;
            }
            else if (str.Length == 0)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], 0);

                offset += Size;
            }
            else
            {
                var written = encoding.GetBytes(str, span.Slice(offset + Size));

                Unsafe.WriteUnaligned(ref bytes[offset], written);

                offset += Size + written;
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

        var offset = Size;
        var span = bytes.AsSpan();

        for (int i = 0; i < value.Count; i++)
        {
            var str = value[i];

            if (str == null)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                offset += Size;
            }
            else if (str.Length == 0)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], 0);

                offset += Size;
            }
            else
            {
                var written = encoding.GetBytes(str, span.Slice(offset + Size));

                Unsafe.WriteUnaligned(ref bytes[offset], written);

                offset += Size + written;
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

        var offset = Size;
        var span = bytes.AsSpan();

        for (int i = 0; i < value.Count; i++)
        {
            var str = value[i];

            if (str == null)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                offset += Size;
            }
            else if (str.Length == 0)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], 0);

                offset += Size;
            }
            else
            {
                var written = encoding.GetBytes(str, span.Slice(offset + Size));

                Unsafe.WriteUnaligned(ref bytes[offset], written);

                offset += Size + written;
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

        var offset = Size;
        var span = bytes.AsSpan();

        foreach (var str in value)
        {
            if (str == null)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                offset += Size;
            }
            else if (str.Length == 0)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], 0);

                offset += Size;
            }
            else
            {
                var written = encoding.GetBytes(str, span.Slice(offset + Size));

                Unsafe.WriteUnaligned(ref bytes[offset], written);

                offset += Size + written;
            }
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

        var offset = Size;
        var span = bytes.AsSpan();

        foreach (var str in value)
        {
            if (str == null)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], int.MaxValue);

                offset += Size;
            }
            else if (str.Length == 0)
            {
                Unsafe.WriteUnaligned(ref bytes[offset], 0);

                offset += Size;
            }
            else
            {
                var written = encoding.GetBytes(str, span.Slice(offset + Size));

                Unsafe.WriteUnaligned(ref bytes[offset], written);

                offset += Size + written;
            }
        }

        return bytes;
    }
}