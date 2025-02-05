using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IT.Redis.Entity.Formatters;

public class UnmanagedFormatter<T> : NullableFormatter<T> where T : unmanaged
{
    public static readonly UnmanagedFormatter<T> Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref T value)
    {
        var span = ((ReadOnlyMemory<byte>)redisValue).Span;

        if (span.Length != Unsafe.SizeOf<T>()) throw Ex.InvalidLength(typeof(T), Unsafe.SizeOf<T>());

        value = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(span));
    }

    public override RedisValue Serialize(in T value)
    {
        var bytes = new byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref bytes[0], value);
        return bytes;
    }
}