namespace IT.Redis.Entity;

public static class xIUtf8Formatter
{
    public static int Format<T>(this IUtf8Formatter<T> formatter, in T value, Span<byte> bytes) =>
        formatter.TryFormat(in value, bytes, out var written) ? written : 0;

    public static int Format<T>(this IUtf8Formatter formatter, in T value, Span<byte> bytes) =>
        formatter.TryFormat(in value, bytes, out var written) ? written : 0;
}