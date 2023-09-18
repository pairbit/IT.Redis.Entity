using System.Collections;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StackExchange.Redis.Entity.Internal;

internal static class Util
{
    public readonly static int RedisValueMaxLength = 512 * 1024 * 1024;
    public readonly static long RedisValueMaxLengthInBits = ((long)RedisValueMaxLength << 3);
    public readonly static long ArrayMaxLengthInBits = ((long)Array.MaxLength << 3);

    public static byte[] GZipHeaderBytes = { 0x1f, 0x8b, 8, 0, 0, 0, 0, 0, 4, 0 };
    public static byte[] GZipLevel10HeaderBytes = { 0x1f, 0x8b, 8, 0, 0, 0, 0, 0, 2, 0 };

    internal static RedisValue SerializeSlow<T>(in T?[]? value) where T : unmanaged
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var size = Unsafe.SizeOf<T>();
        var sizeValue = size * value.Length;
        var bytes = new byte[sizeValue + (value.Length - 1) / 8 + 1];
        var bits = new BitArray(value.Length);
        
        for (int i = 0, b = 0; i < value.Length; i++, b += size)
        {
            var item = value[i];
            if (item == null)
            {
                bits[i] = true;
            }
            else
            {
                Unsafe.WriteUnaligned(ref bytes[b], item.Value);
            }
        }

        bits.CopyTo(bytes, sizeValue);

        return bytes;//40 bytes
    }

    internal static T?[]? DeserializeSlow<T>(in RedisValue redisValue) where T : unmanaged
    {
        if (redisValue == RedisValues.Zero)
        {
            return null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            return Array.Empty<T?>();
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;
            var size = Unsafe.SizeOf<T>();
            //var len = int.MaxValue;
            var length = (int)(((long)span.Length << 3) / ((size << 3) + 1));//span.Length * 8 / ((size * 8) + 1)
            var sizeValue = size * length;

            var bytesBits = new byte[span.Length - sizeValue];
            span.Slice(sizeValue).CopyTo(bytesBits);

            var bits = new BitArray(bytesBits);

            var value = new T?[length];

            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            for (int i = 0, b = 0; i < value.Length; i++, b += size)
            {
                if (bits[i])
                {
                    value[i] = null;
                }
                else
                {
                    value[i] = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref spanRef, b));
                }
            }

            return value;
        }
    }

    public static byte[] Compress(byte[] data, CompressionLevel level = CompressionLevel.Optimal)
    {
        using var output = new MemoryStream();
        using (var dstream = new DeflateStream(output, level))
        {
            dstream.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public static bool IsCompressed(ReadOnlySpan<byte> bytes)
    {
        var yes = bytes.Length > 10;

        if (!yes) return false;

        var header = bytes.Slice(0, 10).ToArray();

        var check = header.SequenceEqual(GZipHeaderBytes) || header.SequenceEqual(GZipLevel10HeaderBytes);

        return true;
    }

    public static byte[] Decompress(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var output = new MemoryStream();
        using (var dstream = new DeflateStream(input, CompressionMode.Decompress))
        {
            dstream.CopyTo(output);
        }
        return output.ToArray();
    }
}