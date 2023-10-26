namespace IT.Redis.Entity.Internal;

internal class Utf8FormatterNotFound<T> : IUtf8Formatter<T>
{
    public int GetLength(in T value)
        => throw Ex.Utf8FormatterNotFound(typeof(T));

    public bool TryFormat(in T value, Span<byte> span, out int written)
        => throw Ex.Utf8FormatterNotFound(typeof(T));
}