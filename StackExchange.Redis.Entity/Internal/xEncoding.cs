#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER)

using System.Text;

namespace StackExchange.Redis.Entity.Internal;

internal static class xEncoding
{
    public static unsafe int GetByteCount(this Encoding encoding, ReadOnlySpan<char> chars)
    {
        fixed (Char* charsPtr = chars)
            return encoding.GetByteCount(charsPtr, chars.Length);
    }

    public static unsafe int GetBytes(this Encoding encoding, string str, Span<byte> bytes)
    {
        fixed (Char* charsPtr = str)
        fixed (Byte* bytesPtr = bytes)
            return encoding.GetBytes(charsPtr, str.Length, bytesPtr, bytes.Length);
    }

    public static unsafe String GetString(this Encoding encoding, ReadOnlySpan<Byte> bytes)
    {
        fixed (Byte* bytesPtr = bytes)
            return encoding.GetString(bytesPtr, bytes.Length);
    }
}

#endif