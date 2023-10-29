namespace IT.Redis.Entity.Internal;

public class Utf8FormatterProxy : IUtf8Formatter
{
    private readonly object _formatter;

    public Utf8FormatterProxy(object formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public int GetLength<T>(in T value)
        => ((IUtf8Formatter<T>)_formatter).GetLength(in value);

    public bool TryFormat<T>(in T value, Span<byte> utf8, out int written)
        => ((IUtf8Formatter<T>)_formatter).TryFormat(in value, utf8, out written);
}