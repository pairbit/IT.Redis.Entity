using System.Text;

namespace IT.Redis.Entity.Utf8Formatters;

public class StringUtf8Formatter : IUtf8Formatter<string>
{
    public static readonly StringUtf8Formatter Default = new();

    public int GetLength(in string value)
        => value == null || value.Length == 0 ? 0 : Encoding.UTF8.GetByteCount(value);

    public bool TryFormat(in string value, Span<byte> span, out int written)
    {
        if (value == null || value.Length == 0)
        {
            written = 0;
            return false;
        }

        written = Encoding.UTF8.GetBytes(value, span);

        return written > 0;
    }
}