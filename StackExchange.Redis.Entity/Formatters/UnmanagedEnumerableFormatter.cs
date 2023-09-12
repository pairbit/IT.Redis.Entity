using StackExchange.Redis.Entity.Internal;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public class UnmanagedEnumerableFormatter<T> :
    IRedisValueFormatter<IEnumerable<T>>,
    IRedisValueFormatter<IReadOnlyCollection<T>>,
    IRedisValueFormatter<IReadOnlyList<T>>, IRedisValueFormatter<IReadOnlyList<T?>>,
    IRedisValueFormatter<IReadOnlySet<T>>,
    IRedisValueFormatter<ICollection<T>>,
    IRedisValueFormatter<IList<T>>, IRedisValueFormatter<IList<T?>>,
    IRedisValueFormatter<ISet<T>>,
    IRedisValueFormatter<T[]>, IRedisValueFormatter<T?[]>,
    IRedisValueFormatter<ReadOnlyCollection<T>>,
    IRedisValueFormatter<Collection<T>>,
    IRedisValueFormatter<List<T>>, IRedisValueFormatter<List<T?>>,
    IRedisValueFormatter<LinkedList<T>>,
    IRedisValueFormatter<HashSet<T>>,
    IRedisValueFormatter<SortedSet<T>>,
    IRedisValueFormatter<Queue<T>>,
    IRedisValueFormatter<Stack<T>>,
    IRedisValueFormatter<ReadOnlyObservableCollection<T>>,
    IRedisValueFormatter<ObservableCollection<T>>
    where T : unmanaged
{
    private static T[]? DeserializeArray(in RedisValue redisValue)
    {
        if (redisValue == RedisValues.Zero) return null;
        if (redisValue == RedisValue.EmptyString) return Array.Empty<T>();

        var span = ((ReadOnlyMemory<byte>)redisValue).Span;
        var size = Unsafe.SizeOf<T>();
        var array = new T[span.Length / size];

        ref byte bytes = ref MemoryMarshal.GetReference(span);

        for (int i = 0, b = 0; i < array.Length; i++, b += size)
        {
            array[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b));
        }

        return array;
    }

    private static T?[]? DeserializeArrayNullable(in RedisValue redisValue)
    {
        if (redisValue == RedisValues.Zero) return null;
        if (redisValue == RedisValue.EmptyString) return Array.Empty<T?>();

        var span = ((ReadOnlyMemory<byte>)redisValue).Span;
        var size = Unsafe.SizeOf<T?>();
        var array = new T?[span.Length / size];

        ref byte bytes = ref MemoryMarshal.GetReference(span);

        for (int i = 0, b = 0; i < array.Length; i++, b += size)
        {
            array[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref bytes, b));
        }

        return array;
    }

    public void Deserialize(in RedisValue redisValue, ref IList<T>? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = new List<T>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T>();
            var length = span.Length / size;
            ref byte bytes = ref MemoryMarshal.GetReference(span);

            if (value == null) value = new List<T>(length);
            else if (value.Count > 0) value.Clear();

            for (int i = 0, b = 0; i < length; i++, b += size)
            {
                value.Add(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b)));
            }
        }
    }

    public void Deserialize(in RedisValue redisValue, ref IReadOnlyList<T>? value) => value = DeserializeArray(in redisValue);

    public void Deserialize(in RedisValue redisValue, ref IEnumerable<T>? value) => value = DeserializeArray(in redisValue);

    public void Deserialize(in RedisValue redisValue, ref IReadOnlyCollection<T>? value) => value = DeserializeArray(in redisValue);

    public void Deserialize(in RedisValue redisValue, ref ICollection<T>? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Array.Empty<T>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T>();
            var count = span.Length / size;
            ref byte bytes = ref MemoryMarshal.GetReference(span);

            if (value == null || value.IsReadOnly)
            {
                var array = value as T[];

                if (array == null || array.Length != count)
                    array = new T[count];

                for (int i = 0, b = 0; i < array.Length; i++, b += size)
                {
                    array[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b));
                }

                value = array;
            }
            else
            {
                if (value.Count > 0) value.Clear();

                for (int i = 0, b = 0; i < count; i++, b += size)
                {
                    value.Add(Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b)));
                }
            }
        }
    }

    public void Deserialize(in RedisValue redisValue, ref ISet<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref IReadOnlySet<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref T[]? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Array.Empty<T>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T>();
            var length = span.Length / size;

            if (value == null || value.Length != length) value = new T[length];

            ref byte bytes = ref MemoryMarshal.GetReference(span);

            for (int i = 0, b = 0; i < value.Length; i++, b += size)
            {
                value[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref bytes, b));
            }
        }
    }

    public void Deserialize(in RedisValue redisValue, ref Collection<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref ReadOnlyCollection<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref List<T>? value)
    {
        var list = (IList<T>?)value;
        Deserialize(in redisValue, ref list);
        value = (List<T>?)list;
    }

    public void Deserialize(in RedisValue redisValue, ref LinkedList<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref Queue<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref Stack<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref HashSet<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref SortedSet<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref ReadOnlyObservableCollection<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref ObservableCollection<T>? value)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(in RedisValue redisValue, ref T?[]? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Array.Empty<T?>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T?>();
            var length = span.Length / size;

            if (value == null || value.Length != length) value = new T?[length];

            ref byte bytes = ref MemoryMarshal.GetReference(span);

            for (int i = 0, b = 0; i < value.Length; i++, b += size)
            {
                value[i] = Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref bytes, b));
            }
        }
    }

    public void Deserialize(in RedisValue redisValue, ref IReadOnlyList<T?>? value) => value = DeserializeArrayNullable(redisValue);

    public void Deserialize(in RedisValue redisValue, ref List<T?>? value)
    {
        var list = (IList<T?>?)value;
        Deserialize(in redisValue, ref list);
        value = (List<T?>?)list;
    }

    public void Deserialize(in RedisValue redisValue, ref IList<T?>? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = new List<T?>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T?>();
            var length = span.Length / size;
            ref byte bytes = ref MemoryMarshal.GetReference(span);

            if (value == null) value = new List<T?>(length);
            else if (value.Count > 0) value.Clear();

            for (int i = 0, b = 0; i < length; i++, b += size)
            {
                value.Add(Unsafe.ReadUnaligned<T?>(ref Unsafe.Add(ref bytes, b)));
            }
        }
    }

    public RedisValue Serialize(in IList<T>? value)
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

    public RedisValue Serialize(in IReadOnlyList<T>? value)
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

    public RedisValue Serialize(in IEnumerable<T>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value is IReadOnlyCollection<T> readOnlyCollection) return Serialize(readOnlyCollection);
        if (value is ICollection<T> collection) return Serialize(collection);
        if (value.TryGetNonEnumeratedCount(out var count))
        {
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
        return Serialize(value.ToArray());
    }

    public RedisValue Serialize(in IReadOnlyCollection<T>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Count == 0) return RedisValue.EmptyString;
        if (value is IReadOnlyList<T> readOnlyList) return Serialize(readOnlyList);
        if (value is IList<T> list) return Serialize(list);

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

    public RedisValue Serialize(in ICollection<T>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Count == 0) return RedisValue.EmptyString;
        if (value is IList<T> list) return Serialize(list);
        if (value is IReadOnlyList<T> readOnlyList) return Serialize(readOnlyList);

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

    public RedisValue Serialize(in ISet<T>? value) => Serialize((ICollection<T>?)value);

    public RedisValue Serialize(in IReadOnlySet<T>? value) => Serialize((IReadOnlyCollection<T>?)value);

    public RedisValue Serialize(in T[]? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var bytes = new byte[size * value.Length];

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    public RedisValue Serialize(in Collection<T>? value) => Serialize((IReadOnlyList<T>?)value);

    public RedisValue Serialize(in ReadOnlyCollection<T>? value) => Serialize((IReadOnlyList<T>?)value);

    public RedisValue Serialize(in List<T>? value) => Serialize((IReadOnlyList<T>?)value);

    public RedisValue Serialize(in LinkedList<T>? value) => Serialize((IReadOnlyCollection<T>?)value);

    public RedisValue Serialize(in Queue<T>? value) => Serialize((IReadOnlyCollection<T>?)value);

    public RedisValue Serialize(in Stack<T>? value) => Serialize((IReadOnlyCollection<T>?)value);

    public RedisValue Serialize(in HashSet<T>? value) => Serialize((IReadOnlyCollection<T>?)value);

    public RedisValue Serialize(in SortedSet<T>? value) => Serialize((IReadOnlyCollection<T>?)value);

    public RedisValue Serialize(in ReadOnlyObservableCollection<T>? value) => Serialize((IReadOnlyList<T>?)value);

    public RedisValue Serialize(in ObservableCollection<T>? value) => Serialize((IReadOnlyList<T>?)value);

    public RedisValue Serialize(in T?[]? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
        var bytes = new byte[size * value.Length];

        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    public RedisValue Serialize(in IReadOnlyList<T?>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
        var bytes = new byte[size * value.Count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }

    public RedisValue Serialize(in List<T?>? value) => Serialize((IReadOnlyList<T?>?)value);

    public RedisValue Serialize(in IList<T?>? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Count == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T?>();
        var bytes = new byte[size * value.Count];

        for (int i = 0, b = 0; i < value.Count; i++, b += size)
        {
            Unsafe.WriteUnaligned(ref bytes[b], value[i]);
        }

        return bytes;
    }
}