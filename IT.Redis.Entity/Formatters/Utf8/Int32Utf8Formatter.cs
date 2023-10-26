using System.Buffers.Text;

namespace IT.Redis.Entity.Formatters.Utf8;

public class Int32Utf8Formatter : IUtf8Formatter<Int32>
{
    public static readonly Int32Utf8Formatter Default = new();

    public int GetLength(in int value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
        if (value <= 9) return 1;

        return (int)Math.Floor(Math.Log10(value)) + 1;
    }

    public bool TryFormat(in int value, Span<byte> span, out int written)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
        return Utf8Formatter.TryFormat(value, span, out written);
    }
}