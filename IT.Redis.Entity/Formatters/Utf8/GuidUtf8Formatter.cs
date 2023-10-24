using System.Buffers;
using System.Buffers.Text;

namespace IT.Redis.Entity.Formatters.Utf8;

public class GuidUtf8Formatter : IUtf8Formatter<Guid>
{
    public static readonly GuidUtf8Formatter Default = new();

    public int GetLength(in Guid value) => 32;

    public bool TryFormat(in Guid value, Span<byte> span, out int written)
        => Utf8Formatter.TryFormat(value, span, out written, new StandardFormat('N'));
}