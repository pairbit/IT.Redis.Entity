using IT.Redis.Entity.Internal;
using System.Text;

namespace IT.Redis.Entity.Formatters.Utf8;

public class StringUtf8Formatter : IUtf8Formatter<string>
{
    public static readonly StringUtf8Formatter Default = new();

    public int GetLength(in string value)
        => value == null ? 0 : Encoding.UTF8.GetByteCount(value);

    public bool TryFormat(in string value, Span<byte> span, out int written)
    {
        if (value == null)
        {
            written = 0;
            return false;
        }

        written = Encoding.UTF8.GetBytes(value, span);

        return written > 0;
    }
}