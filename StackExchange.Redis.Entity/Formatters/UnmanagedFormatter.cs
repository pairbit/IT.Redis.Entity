﻿using StackExchange.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Formatters;

public class UnmanagedFormatter<T> : NullableFormatter<T> where T : unmanaged
{
    public override void DeserializeNotNull(in RedisValue redisValue, ref T value)
    {
        var span = ((ReadOnlyMemory<byte>)redisValue).Span;

        if (span.Length != Unsafe.SizeOf<T>()) throw Ex.InvalidLength(typeof(T), Unsafe.SizeOf<T>());

        ref byte spanRef = ref MemoryMarshal.GetReference(span);

        value = Unsafe.ReadUnaligned<T>(ref spanRef);
    }

    public override RedisValue Serialize(in T value)
    {
        var bytes = new byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref bytes[0], value);
        return bytes;
    }
}