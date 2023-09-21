using System.Collections.Generic;

namespace DocLib;

public record SimpleRecord
{
    public decimal Decimal { get; set; }

    public DateTimeKind DateTimeKind { get; set; }

    public DocumentSize Size { get; set; }

    public DocumentSizeLong SizeLong { get; set; }

    public string?[]? Strings { get; set; }

    public IReadOnlyCollection<string?>? StringCollection { get; set; }

    public ICollection<KeyValuePair<int,int>> KeyValuePairs { get; set; }

    public IDictionary<int, int> Dictionary { get; set; }

    public DocumentVersionInfoDictionary Versions { get; set; }

    //public IDictionary<string, string> StringDictionary { get; set; }

#if NETCOREAPP3_1_OR_GREATER

    public System.Collections.Immutable.IImmutableStack<int?> ImmutableList { get; set; }

#endif
}