using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public class UnmanagedEnumerableNullableFormatter<TEnumerable, T> : IRedisValueFormatter<TEnumerable>
    where TEnumerable : IEnumerable<T?>
    where T : unmanaged
{
    private readonly IEnumerableFactory _factory;

    public UnmanagedEnumerableNullableFormatter(IEnumerableFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public void Deserialize(in RedisValue redisValue, ref TEnumerable? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = default;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = New(0);
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T?>();
            var length = span.Length / size;
            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            if (value == null)
            {
                var newEnumerable = New(length);

                if (newEnumerable is T?[] array)
                {
                    for (int i = 0, b = 0; i < array.Length; i++, b += size)
                    {
                        array[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b));
                    }
                }
                else if (newEnumerable is ICollection<T?> collection)
                {
                    for (int i = 0, b = 0; i < length; i++, b += size)
                    {
                        collection.Add(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                    }
                }
                else if (newEnumerable is Stack<T?> stack)
                {
                    for (int i = 0, b = 0; i < length; i++, b += size)
                    {
                        stack.Push(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                    }
                }
                else
                {
                    throw new NotImplementedException($"{newEnumerable.GetType().FullName} not implemented add");
                }

                value = newEnumerable;
            }
            else
            {
                if (value is T?[] array)
                {
                    if (array.Length != length)
                    {
                        array = new T?[length];
                        value = (TEnumerable)(object)array;
                    }

                    for (int i = 0, b = 0; i < array.Length; i++, b += size)
                    {
                        array[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b));
                    }
                }
                else if (value is List<T?> list)
                {
                    var count = list.Count;
                    if (count < length)
                    {
                        if (list.Capacity < length) list.Capacity = length;

                        var b = 0;

                        for (int i = 0; i < list.Count; i++, b += size)
                        {
                            list[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b));
                        }

                        count = length - count;

                        for (int i = 0; i < count; i++, b += size)
                        {
                            list.Add(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                        }
                    }
                    else
                    {
                        for (int i = 0, b = 0; i < length; i++, b += size)
                        {
                            list[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b));
                        }

                        count -= length;

                        for (int i = count - 1; i >= 0; i--)
                        {
                            list.RemoveAt(i);
                        }
                    }
                }
                else if (value is ICollection<T?> collection)
                {
                    if (collection.Count > 0) collection.Clear();

                    for (int i = 0, b = 0; i < length; i++, b += size)
                    {
                        collection.Add(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref spanRef, b)));
                    }
                }
                else if (value is Stack<T?> stack)
                {
                    //TODO: возможно без Clear?
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

    public RedisValue Serialize(in TEnumerable? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value is T?[] array) return SerializeArray(array);
        if (value is IReadOnlyList<T?> readOnlyList) return SerializeReadOnlyList(readOnlyList);
        if (value is IList<T?> list) return SerializeList(list);
        if (value is IReadOnlyCollection<T?> readOnlyCollection) return SerializeReadOnlyCollection(readOnlyCollection);
        if (value is ICollection<T?> collection) return SerializeCollection(collection);
        if (value.TryGetNonEnumeratedCount(out var count))
        {
            if (count == 0) return RedisValue.EmptyString;

            var size = Unsafe.SizeOf<T?>();
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

    private TEnumerable New(int capacity) => _factory.New<TEnumerable, T?>(capacity);

    private static RedisValue SerializeArray(in T?[] value)
    {
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
        var bytes = new byte[size * value.Length];

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyList(in IReadOnlyList<T?> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
        var bytes = new byte[size * value.Count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeList(in IList<T?> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
        var bytes = new byte[size * value.Count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    private static RedisValue SerializeReadOnlyCollection(in IReadOnlyCollection<T?> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
        var bytes = new byte[size * value.Count];
        int b = 0;

        foreach (var item in value)
        {
            Unsafe.WriteUnaligned(ref bytes[b], item);
            b += size;
        }

        return bytes;
    }

    private static RedisValue SerializeCollection(in ICollection<T?> value)
    {
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
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