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
        Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<Guid>()];
        
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(bytes), value);

        return Base64.EncodeToUtf8(bytes, utf8, out var consumed, out written) == OperationStatus.Done;
    }
}