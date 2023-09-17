using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public class UnmanagedEnumerableNullableFormatter<TEnumerable, T> : IRedisValueFormatter<TEnumerable>
    where TEnumerable : IEnumerable<T?>
    where T : unmanaged
{
    private readonly IEnumerableFactory<TEnumerable, T?> _factory;

    public UnmanagedEnumerableNullableFormatter(IEnumerableFactory<TEnumerable, T?> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public UnmanagedEnumerableNullableFormatter(EnumerableFactory<TEnumerable, T?> factory)
    {
        _factory = new EnumerableFactoryDelegate<TEnumerable, T?>(factory ?? throw new ArgumentNullException(nameof(factory)));
    }

    public void Deserialize(in RedisValue redisValue, ref TEnumerable? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = default;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = _factory.Empty();
        }
        else
        {
            var memory = (ReadOnlyMemory<byte>)redisValue;
            var span = memory.Span;
            var size = Unsafe.SizeOf<T?>();
            var length = span.Length / size;

            if (value == null) value = _factory.New(length, in memory, UnmanagedEnumerableNullableFormatter.Build);
            else
            {
                ref byte spanRef = ref MemoryMarshal.GetReference(span);

                if (value is T?[] array)
                {
                    if (array.Length != length)
                    {
                        array = new T?[length];
                        value = (TEnumerable)(IEnumerable<T?>)array;
                    }

                    for (int i = 0, b = 0; i < array.Length; i++, b += size)
                    {
                        array[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b));
                    }
                }
                else if (value is ICollection<T?> collection)
                {
                    if (collection.IsReadOnly)
                    {
                        value = _factory.New(length, in memory, UnmanagedEnumerableNullableFormatter.Build);
                    }
                    else if (value is IList<T?> ilist)
                    {
                        var count = ilist.Count;
                        if (count < length)
                        {
                            if (ilist is List<T?> list && list.Capacity < length) list.Capacity = length;

                            var b = 0;

                            for (int i = 0; i < ilist.Count; i++, b += size)
                            {
                                ilist[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b));
                            }

                            count = length - count;

                            for (int i = 0; i < count; i++, b += size)
                            {
                                ilist.Add(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                            }
                        }
                        else
                        {
                            for (int i = 0, b = 0; i < length; i++, b += size)
                            {
                                ilist[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b));
                            }

                            count -= length;

                            for (int i = count - 1; i >= 0; i--)
                            {
                                ilist.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        if (collection.Count > 0) collection.Clear();

                        for (int i = 0, b = 0; i < length; i++, b += size)
                        {
                            collection.Add(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                        }
                    }
                }
                else if (value is Queue<T?> queue)
                {
                    if (queue.Count > 0) queue.Clear();

                    queue.EnsureCapacity(length);

                    for (int i = 0, b = 0; i < length; i++, b += size)
                    {
                        queue.Enqueue(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                    }
                }
                else if (value is Stack<T?> stack)
                {
                    if (stack.Count > 0) stack.Clear();

                    stack.EnsureCapacity(length);

                    for (int i = 0, b = 0; i < length; i++, b += size)
                    {
                        stack.Push(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                    }
                }
                else
                {
                    throw new NotImplementedException($"{value.GetType().FullName} not implemented add");
                }
            }
        }
    }

    public RedisValue Serialize(in TEnumerable? value) => UnmanagedEnumerableNullableFormatter.Serialize(value);
}