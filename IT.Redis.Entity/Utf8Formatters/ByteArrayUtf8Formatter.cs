namespace IT.Redis.Entity.Utf8Formatters;

public class ByteArrayUtf8Formatter : IUtf8Formatter<byte[]>
{
    public static readonly ByteArrayUtf8Formatter Default = new();

    public int GetLength(in byte[] value) => value == null ? 0 : value.Length;

    public bool TryFormat(in byte[] value, Span<byte> span, out int written)
    {
        if (value == null || value.Length == 0)
        {
            written = 0;
            return false;
        }

        value.CopyTo(span);
        written = value.Length;
        return true;
    }
}