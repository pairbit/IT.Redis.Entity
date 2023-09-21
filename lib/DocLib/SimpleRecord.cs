namespace DocLib;

public record SimpleRecord
{
    public decimal Decimal { get; set; }

    public DateTimeKind DateTimeKind { get; set; }

    public DocumentSize Size { get; set; }

    public DocumentSizeLong SizeLong { get; set; }

    public string?[]? Strings { get; set; }

    //public IList<string?>? StringList { get; set; }
}