using System.Collections;
using System.Numerics;

namespace DocLib;

public interface IDocument : IReadOnlyDocument
{
    public new Guid Id { get; set; }

    public new Guid ExternalId { get; set; }

    public new string? Name { get; set; }

    public new char Character { get; set; }

    public new DateOnly StartDate { get; set; }

    public new DateOnly? EndDate { get; set; }

    public new long Price { get; set; }

    public new bool IsDeleted { get; set; }

    public new DocumentSize Size { get; set; }

    public new DateTime Created { get; set; }

    public new DateTime? Modified { get; set; }

    public new byte[]? Content { get; set; }

    public new ReadOnlyMemory<byte>? MemoryBytes { get; set; }

    public new BigInteger BigInteger { get; set; }

    public new BitArray? Bits { get; set; }

    public new Version? Version { get; set; }

    public new Uri? Url { get; set; }

    public new int[]? IntArray { get; set; }

    public new int?[]? IntArrayN { get; set; }
}