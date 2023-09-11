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
}