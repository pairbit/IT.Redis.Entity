using System.Collections.Concurrent;

namespace DocLib;

public record SimpleRecord
{
    public decimal Decimal { get; set; }

    public DateTimeKind DateTimeKind { get; set; }

    public DocumentSize Size { get; set; }

    public DocumentSizeLong SizeLong { get; set; }

    public string?[]? Strings { get; set; }

    public IReadOnlyCollection<string?>? StringCollection { get; set; }

    public ICollection<KeyValuePair<int, int?>> KeyValuePairs { get; set; }

    public IDictionary<int, int> Dictionary { get; set; }

    public IReadOnlyDictionary<int, int?> ReadOnlyDictionary { get; set; }

    public DocumentVersionInfoDictionary Versions { get; set; }

    public SortedDictionary<int, int> SortedDictionary { get; set; }

    public SortedList<int, int?> SortedList { get; set; }

    public ConcurrentDictionary<int, int> ConcurrentDictionary { get; set; }

    public IProducerConsumerCollection<int> ProducerConsumerCollection { get; set; }

    public ConcurrentBag<int> ConcurrentBag { get; set; }

    public ConcurrentQueue<int> ConcurrentQueue { get; set; }

    public ConcurrentStack<int> ConcurrentStack { get; set; }

    public BlockingCollection<int> BlockingCollection { get; set; }

    //public IDictionary<string, string> StringDictionary { get; set; }

#if NETCOREAPP3_1_OR_GREATER

    public System.Collections.Immutable.IImmutableStack<int?> ImmutableList { get; set; }

    public (int, int) Tuple { get; set; }

    public (int?, int?) NullableTuple { get; set; }

#endif
}