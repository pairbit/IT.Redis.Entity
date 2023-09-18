using System.Collections;
using System.Numerics;

namespace DocLib;

public class ReadOnlyDocument : IReadOnlyDocument
{
    private readonly IReadOnlyDocument _document;

    public ReadOnlyDocument(IReadOnlyDocument document)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
    }

    public Guid Id => _document.Id;

    public Guid ExternalId => _document.ExternalId;

    public string? Name => _document.Name;

    public char Character => _document.Character;

    public DateOnly StartDate => _document.StartDate;

    public DateOnly? EndDate => _document.EndDate;

    public long Price => _document.Price;

    public bool IsDeleted => _document.IsDeleted;

    public DocumentSize Size => _document.Size;

    public DateTime Created => _document.Created;

    public DateTime? Modified => _document.Modified;

    public byte[]? Content => _document.Content;

    public ReadOnlyMemory<byte>? MemoryBytes => _document.MemoryBytes;

    public BigInteger BigInteger => _document.BigInteger;

    public BitArray? Bits => _document.Bits;

    public Version? Version => _document.Version;

    public Uri? Url => _document.Url;

    public int[]? IntArray => _document.IntArray;

    public int?[]? IntArrayN => _document.IntArrayN;

    public IReadOnlyList<Guid?>? TagIds => _document.TagIds;

    public DocumentVersionInfo? VersionInfo => _document.VersionInfo;

    public DocumentVersionInfos? VersionInfos => _document.VersionInfos;

    public IEnumerable<decimal?>? Decimals => _document.Decimals;
}