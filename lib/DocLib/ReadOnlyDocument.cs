using System.Runtime.Serialization;

namespace DocLib;

public record ReadOnlyDocument
{
    private readonly Document _document;

    public ReadOnlyDocument(Document document)
    {
        _document = document;
    }

    [DataMember(Order = 0)]
    public string Name => _document.Name;

    [DataMember(Order = 1)]
    public DateOnly StartDate => _document.StartDate;

    [DataMember(Order = 2)]
    public DateOnly? EndDate => _document.EndDate;

    [DataMember(Order = 3)]
    public long Price => _document.Price;

    [DataMember(Order = 4)]
    public bool IsDeleted => _document.IsDeleted;

    [DataMember(Order = 5)]
    public DocumentSize Size => _document.Size;
}