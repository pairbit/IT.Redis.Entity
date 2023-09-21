namespace DocLib;

public record SimpleRecord
{
    public decimal Decimal { get; set; }

    public DateTimeKind DateTimeKind { get; set; }

    public DocumentSize Size { get; set; }

    public DocumentSizeLong SizeLong { get; set; }

    public string?[]? Strings { get; set; }

    public IReadOnlyCollection<string?>? StringCollection { get; set; }

#if NETCOREAPP3_1_OR_GREATER

    public System.Collections.Immutable.ImmutableArray<int?> ImmutableList { get; set; }

#endif
}