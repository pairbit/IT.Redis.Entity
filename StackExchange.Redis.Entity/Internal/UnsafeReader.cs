using System.Runtime.CompilerServices;
using System.Text;

namespace StackExchange.Redis.Entity.Internal;

internal static class UnsafeReader
{
    private const int Size = sizeof(int);

    public static string? ReadString(ref byte refLen, ref ReadOnlySpan<byte> spanStr, Encoding encoding)
    {
        var len = Unsafe.ReadUnaligned<int>(ref refLen);

        if (len == int.MaxValue) return null;

        if (len == 0) return string.Empty;

        var str = encoding.GetString(spanStr.Slice(0, len));

        spanStr = spanStr.Slice(len);

        return str;
    }

    public static string? ReadStringFromEnd(ref byte refLen, ref ReadOnlySpan<byte> spanStr, Encoding encoding)
    {
        var len = Unsafe.ReadUnaligned<int>(ref refLen);

        if (len == int.MaxValue) return null;

        if (len == 0) return string.Empty;

        len = spanStr.Length - len;

        var str = encoding.GetString(spanStr.Slice(len));

        spanStr = spanStr.Slice(0, len);

        return str;
    }

    public static KeyValuePair<string?, string?> ReadPairString(ref byte refLen, ref ReadOnlySpan<byte> spanStr, Encoding encoding)
        => new(ReadString(ref refLen, ref spanStr, encoding), ReadString(ref Unsafe.Add(ref refLen, Size), ref spanStr, encoding));

    public static KeyValuePair<string?, string?> ReadPairStringFromEnd(ref byte refLen, ref ReadOnlySpan<byte> spanStr, Encoding encoding)
    {
        var val = ReadStringFromEnd(ref refLen, ref spanStr, encoding);
        var key = ReadStringFromEnd(ref Unsafe.Add(ref refLen, -Size), ref spanStr, encoding);
        return new(key, val);
    }
}