using System.Collections;
using System.Numerics;
using System.Runtime.Serialization;

namespace DocLib;

[DataContract]
public class DocumentDataContract : IDocument
{
    [DataMember(Order = 0)]
    public Guid Id { get; set; }

    [DataMember(Order = 1)]
    public Guid ExternalId { get; set; }

    [DataMember(Order = 2)]
    public string? Name { get; set; }

    [DataMember(Order = 3)]
    public char Character { get; set; }

    [DataMember(Order = 4)]
    public DateOnly StartDate { get; set; }

    [DataMember(Order = 5)]
    public DateOnly? EndDate { get; set; }

    [DataMember(Order = 6)]
    public long Price { get; set; }

    [DataMember(Order = 7)]
    public bool IsDeleted { get; set; }

    [DataMember(Order = 8)]
    public DocumentSize Size { get; set; }

    [DataMember(Order = 9)]
    public DateTime Created { get; set; }

    [DataMember(Order = 10)]
    public DateTime? Modified { get; set; }

    [DataMember(Order = 11)]
    public byte[]? Content { get; set; }

    [DataMember(Order = 12)]
    public ReadOnlyMemory<byte>? MemoryBytes { get; set; }

    [DataMember(Order = 13)]
    public BigInteger BigInteger { get; set; }

    [DataMember(Order = 14)]
    public BitArray? Bits { get; set; }

    [DataMember(Order = 15)]
    public Version? Version { get; set; }

    [DataMember(Order = 16)]
    public Uri? Url { get; set; }

    [DataMember(Order = 17)]
    public int[]? IntArray { get; set; }

    [DataMember(Order = 18)]
    public int?[]? IntArrayN { get; set; }

    [DataMember(Order = 19)]
    public List<Guid?>? TagIds { get; set; }

    IReadOnlyList<Guid?>? IReadOnlyDocument.TagIds => TagIds;
}