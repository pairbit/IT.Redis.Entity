using System.Collections;
using System.Numerics;

namespace DocLib;

public record DocumentPOCO : IDocument
{
    public Guid Id { get; set; }

    public Guid ExternalId { get; set; }

    public string? Name { get; set; }

    public char Character { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public long Price { get; set; }

    public bool IsDeleted { get; set; }

    public DocumentSize Size { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public byte[]? Content { get; set; }

    public ReadOnlyMemory<byte>? MemoryBytes { get; set; }

    public BigInteger BigInteger { get; set; }

    public BitArray? Bits { get; set; }

    public Version? Version { get; set; }

    public Uri? Url { get; set; }

    public int[]? IntArray { get; set; }

    public int?[]? IntArrayN { get; set; }

    public List<Guid?>? TagIds { get; set; }

    IReadOnlyList<Guid?>? IReadOnlyDocument.TagIds => TagIds?.AsReadOnly();
}