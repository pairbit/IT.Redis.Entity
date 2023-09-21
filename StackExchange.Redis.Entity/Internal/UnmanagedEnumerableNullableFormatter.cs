using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Internal;

internal static class UnmanagedEnumerableNullableFormatter
{
    internal static void Build<T>(IEnumerable<T?> buffer, in ReadOnlyMemory<byte> memory) where T : unmanaged
    {
        var span = memory.Span;
        var size = Unsafe.SizeOf<T>();
        var count = (int)(((long)span.Length << 3) / ((size << 3) + 1));
        ref byte spanRef = ref MemoryMarshal.GetReference(span);

        var iBits = 0;
        var iBytes = size * count;
        var bits = span[iBytes];
        int i = 0, b = 0;

        if (buffer is T?[] array)
        {
            //if (array.Length < count) throw new InvalidOperationException();

            do
            {
                if ((bits & (1 << iBits)) == 0) array[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));

                if (++i == array.Length) break;

                b += size;

                if (++iBits == 8)
                {
                    bits = span[++iBytes];
                    iBits = 0;
                }
            } while (true);
        }
        else if (buffer is ICollection<T?> collection)
        {
            do
            {
                collection.Add((bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null);

                if (++i == count) break;

                b += size;

                if (++iBits == 8)
                {
                    bits = span[++iBytes];
                    iBits = 0;
                }
            } while (true);
        }
        else if (buffer is Queue<T?> queue)
        {
            do
            {
                queue.Enqueue((bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null);

                if (++i == count) break;

                b += size;

                if (++iBits == 8)
                {
                    bits = span[++iBytes];
                    iBits = 0;
                }
            } while (true);
        }
        else if (buffer is Stack<T?> stack)
        {
            spanRef = ref Unsafe.Add(ref spanRef, iBytes - size);
            iBits = (count % 8) - 1;
            if (iBits == -1) iBits = 7;
            iBytes = span.Length - 1;
            bits = span[iBytes];
            i = count;

            do
            {
                stack.Push((bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, -b)) : null);

                if (--i == 0) break;

                if (--iBits == -1)
                {
                    bits = span[--iBytes];
                    iBits = 7;
                }

                b += size;
            } while (true);
        }
        else
        {
            throw new NotImplementedException($"{buffer.GetType().FullName} not implemented add");
        }
    }

    internal static bool Deserialize<T>(ref IEnumerable<T?> value, in ReadOnlySpan<byte> span, int size, int length) where T : unmanaged
    {
        ref byte spanRef = ref MemoryMarshal.GetReference(span);

        var iBits = 0;
        var iBytes = size * length;
        var bits = span[iBytes];
        int i = 0, b = 0;

        if (value is T?[] array)
        {
            if (array.Length != length)
            {
                array = new T?[length];
                value = array;
            }

            do
            {
                array[i] = (bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null;

                if (++i == array.Length) break;

                b += size;

                if (++iBits == 8)
                {
                    bits = span[++iBytes];
                    iBits = 0;
                }
            } while (true);
        }
        else if (value is ICollection<T?> collection)
        {
            if (collection.IsReadOnly) return false;
            else if (value is IList<T?> ilist)
            {
                var count = ilist.Count;
                if (count < length)
                {
                    if (ilist is List<T?> list && list.Capacity < length) list.Capacity = length;

                    if (count > 0)
                    {
                        do
                        {
                            ilist[i] = (bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null;

                            b += size;

                            if (++iBits == 8)
                            {
                                bits = span[++iBytes];
                                iBits = 0;
                            }
                        } while (++i < ilist.Count);
                    }

                    do
                    {
                        ilist.Add((bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null);

                        if (++i == length) break;

                        b += size;

                        if (++iBits == 8)
                        {
                            bits = span[++iBytes];
                            iBits = 0;
                        }
                    } while (true);
                }
                else
                {
                    do
                    {
                        ilist[i] = (bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null;

                        if (++i == length) break;

                        b += size;

                        if (++iBits == 8)
                        {
                            bits = span[++iBytes];
                            iBits = 0;
                        }
                    } while (true);

                    i = count - 1;

                    for (; i >= length; i--) ilist.RemoveAt(i);
                }
            }
            else
            {
                if (collection.Count > 0) collection.Clear();

                do
                {
                    collection.Add((bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null);

                    if (++i == length) break;

                    b += size;

                    if (++iBits == 8)
                    {
                        bits = span[++iBytes];
                        iBits = 0;
                    }
                } while (true);
            }
        }
        else if (value is Queue<T?> queue)
        {
            if (queue.Count > 0) queue.Clear();

#if NET6_0_OR_GREATER
            queue.EnsureCapacity(length);
#endif
            do
            {
                queue.Enqueue((bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b)) : null);

                if (++i == length) break;

                b += size;

                if (++iBits == 8)
                {
                    bits = span[++iBytes];
                    iBits = 0;
                }
            } while (true);
        }
        else if (value is Stack<T?> stack)
        {
            if (stack.Count > 0) stack.Clear();

#if NET6_0_OR_GREATER
            stack.EnsureCapacity(length);
#endif
            spanRef = ref Unsafe.Add(ref spanRef, iBytes - size);
            iBits = (length % 8) - 1;
            if (iBits == -1) iBits = 7;
            iBytes = span.Length - 1;
            bits = span[iBytes];
            i = length;
            
            do
            {
                stack.Push((bits & (1 << iBits)) == 0 ? Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, -b)) : null);

                if (--i == 0) break;

                if (--iBits == -1)
                {
                    bits = span[--iBytes];
                    iBits = 7;
                }

                b += size;
            } while (true);
        }
        else
        {
            throw new NotImplementedException($"{value.GetType().FullName} not implemented add");
        }

        return true;
    }

    internal static RedisValue Serialize<T>(in IEnumerable<T?>? value) where T : unmanaged
    {
        if (value == null) return RedisValues.Zero;
        if (value is T?[] array) return SerializeArray(array);
        if (value is IReadOnlyList<T?> readOnlyList) return SerializeReadOnlyList(readOnlyList);
        if (value is IList<T?> list) return SerializeList(list);
        if (value is IReadOnlyCollection<T?> readOnlyCollection) return SerializeReadOnlyCollection(readOnlyCollection);
        if (value is ICollection<T?> collection) return SerializeCollection(collection);
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
            var maxLength = (int)(Util.RedisValueMaxLengthInBits / ((size << 3) + 1));
            if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

            var sizeValue = size * count;
            var bytes = new byte[sizeValue + (count - 1) / 8 + 1];

            byte bits = 0;
            var iBits = 0;
            var iBytes = sizeValue;
            int b = 0;

            foreach (var item in value)
            {
                if (item == null)
                {
                    bits = (byte)(bits | (1 << iBits));
                }
                else
                {
                    Unsafe.WriteUnaligned(ref bytes[b], item.Value);
                }
                if (++iBits == 8)
                {
                    bytes[iBytes++] = bits;
                    iBits = 0;
                    bits = 0;
                }
                b += size;
            }

            if (iBits > 0) bytes[iBytes] = bits;

            return bytes;
        }
        return SerializeArray(value.ToArray());
    }

    internal static RedisValue SerializeArray<T>(in T?[] value) where T : unmanaged
    {
        var length = value.Length;
        if (length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = (int)(Util.RedisValueMaxLengthInBits / ((size << 3) + 1));
        if (length > maxLength) throw Ex.InvalidLengthCollection(typeof(T), length, maxLength);

        var sizeValue = size * length;
        var bytes = new byte[sizeValue + (length - 1) / 8 + 1];

        byte bits = 0;
        var iBits = 0;
        var iBytes = sizeValue;

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            var item = value[i];
            if (item == null)
            {
                bits = (byte)(bits | (1 << iBits));
            }
            else
            {
                Unsafe.WriteUnaligned(ref bytes[b], item.Value);
            }
            if (++iBits == 8)
            {
                bytes[iBytes++] = bits;
                iBits = 0;
                bits = 0;
            }
        }

        if (iBits > 0) bytes[iBytes] = bits;

        return bytes;
    }

    private static RedisValue SerializeReadOnlyList<T>(in IReadOnlyList<T?> value) where T : unmanaged
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = (int)(Util.RedisValueMaxLengthInBits / ((size << 3) + 1));
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var sizeValue = size * count;
        var bytes = new byte[sizeValue + (count - 1) / 8 + 1];

        byte bits = 0;
        var iBits = 0;
        var iBytes = sizeValue;

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            var item = value[i];
            if (item == null)
            {
                bits = (byte)(bits | (1 << iBits));
            }
            else
            {
                Unsafe.WriteUnaligned(ref bytes[b], item.Value);
            }
            if (++iBits == 8)
            {
                bytes[iBytes++] = bits;
                iBits = 0;
                bits = 0;
            }
        }

        if (iBits > 0) bytes[iBytes] = bits;

        return bytes;
    }

    private static RedisValue SerializeList<T>(in IList<T?> value) where T : unmanaged
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = (int)(Util.RedisValueMaxLengthInBits / ((size << 3) + 1));
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var sizeValue = size * count;
        var bytes = new byte[sizeValue + (count - 1) / 8 + 1];

        byte bits = 0;
        var iBits = 0;
        var iBytes = sizeValue;

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            var item = value[i];
            if (item == null)
            {
                bits = (byte)(bits | (1 << iBits));
            }
            else
            {
                Unsafe.WriteUnaligned(ref bytes[b], item.Value);
            }
            if (++iBits == 8)
            {
                bytes[iBytes++] = bits;
                iBits = 0;
                bits = 0;
            }
        }

        if (iBits > 0) bytes[iBytes] = bits;

        return bytes;
    }

    private static RedisValue SerializeReadOnlyCollection<T>(in IReadOnlyCollection<T?> value) where T : unmanaged
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = (int)(Util.RedisValueMaxLengthInBits / ((size << 3) + 1));
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var sizeValue = size * count;
        var bytes = new byte[sizeValue + (count - 1) / 8 + 1];

        byte bits = 0;
        var iBits = 0;
        var iBytes = sizeValue;
        int b = 0;

        foreach (var item in value)
        {
            if (item == null)
            {
                bits = (byte)(bits | (1 << iBits));
            }
            else
            {
                Unsafe.WriteUnaligned(ref bytes[b], item.Value);
            }
            if (++iBits == 8)
            {
                bytes[iBytes++] = bits;
                iBits = 0;
                bits = 0;
            }
            b += size;
        }

        if (iBits > 0) bytes[iBytes] = bits;

        return bytes;
    }

    private static RedisValue SerializeCollection<T>(in ICollection<T?> value) where T : unmanaged
    {
        var count = value.Count;
        if (count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var maxLength = (int)(Util.RedisValueMaxLengthInBits / ((size << 3) + 1));
        if (count > maxLength) throw Ex.InvalidLengthCollection(typeof(T), count, maxLength);

        var sizeValue = size * count;
        var bytes = new byte[sizeValue + (count - 1) / 8 + 1];

        byte bits = 0;
        var iBits = 0;
        var iBytes = sizeValue;
        int b = 0;

        foreach (var item in value)
        {
            if (item == null)
            {
                bits = (byte)(bits | (1 << iBits));
            }
            else
            {
                Unsafe.WriteUnaligned(ref bytes[b], item.Value);
            }
            if (++iBits == 8)
            {
                bytes[iBytes++] = bits;
                iBits = 0;
                bits = 0;
            }
            b += size;
        }

        if (iBits > 0) bytes[iBytes] = bits;

        return bytes;
    }
}