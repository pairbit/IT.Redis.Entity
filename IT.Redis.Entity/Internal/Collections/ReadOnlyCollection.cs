using System.Collections;

namespace IT.Redis.Entity.Internal.Collections;

internal class ReadOnlyCollection<T> : IReadOnlyCollection<T>
{
    private readonly IReadOnlyCollection<T> _collection;
    public readonly static ReadOnlyCollection<T> Empty = new(new LinkedList<T>());

    public ReadOnlyCollection(IReadOnlyCollection<T> collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public int Count => _collection.Count;

    public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collection).GetEnumerator();
}