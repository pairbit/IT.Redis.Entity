using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IT.Redis.Entity.Utf8Formatters;

public class GuidBase64Utf8Formatter : IUtf8Formatter<Guid>
{
    public static readonly GuidBase64Utf8Formatter Default = new();

    public int GetLength(in Guid value) => 22;

    public bool TryFormat(in Guid value, Span<byte> utf8, out int written)
    {
        if (utf8.Length < 22)
        {
            written = 0;
            return false;
        }

        Span<byte> bytes = stackalloc byte[24];
        
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(bytes), value);

        var status = Base64.EncodeToUtf8InPlace(bytes, 16, out var bytesWritten);

        written = 22;

        if (status != OperationStatus.Done || bytesWritten != 24) return false;

        bytes.Slice(0, written).CopyTo(utf8);

        return true;
    }
}