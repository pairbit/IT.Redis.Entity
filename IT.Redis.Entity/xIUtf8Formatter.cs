namespace IT.Redis.Entity;

public static class xIUtf8Formatter
{
    public static int Format<T>(this IUtf8Formatter<T> formatter, in T value, Span<byte> span) =>
        formatter.TryFormat(in value, span, out var written) ? written : 0;
}