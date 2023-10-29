using System.Buffers;
using System.Buffers.Text;

namespace IT.Redis.Entity.Utf8Formatters;

public class GuidHexUtf8Formatter : IUtf8Formatter<Guid>
{
    public static readonly GuidHexUtf8Formatter Default = new();

    public int GetLength(in Guid value) => 32;

    public bool TryFormat(in Guid value, Span<byte> utf8, out int written)
        => Utf8Formatter.TryFormat(value, utf8, out written, new StandardFormat('N'));
}