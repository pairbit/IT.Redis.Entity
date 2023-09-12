using System.Collections;
using System.Numerics;

namespace DocLib;

public interface IReadOnlyDocument
{
    public Guid Id { get; }

    public Guid ExternalId { get; }

    public string? Name { get; }

    public char Character { get; }

    public DateOnly StartDate { get; }

    public DateOnly? EndDate { get; }

    public long Price { get; }

    public bool IsDeleted { get; }

    public DocumentSize Size { get; }

    public DateTime Created { get; }

    public DateTime? Modified { get; }

    public byte[]? Content { get; }

    public ReadOnlyMemory<byte>? MemoryBytes { get; }

    public BigInteger BigInteger { get; }

    public BitArray? Bits { get; }

    public Version? Version { get; }

    public Uri? Url { get; }

    public int[]? IntArray { get; }

    public int?[]? IntArrayN { get; }

    public IReadOnlyList<Guid?>? TagIds { get; }
}