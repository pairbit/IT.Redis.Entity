using System.Collections.ObjectModel;

namespace IT.Redis.Entity.Factories;

public class ReadOnlyObservableCollectionFactory : IEnumerableFactory
{
    public static readonly ReadOnlyObservableCollectionFactory Default = new();

    public IEnumerable<T> Empty<T>() => Cache<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return Cache<T>.Empty;

        var list = new List<T>(capacity);

        builder(list, in state);

        return new ReadOnlyObservableCollection<T>(new ObservableCollection<T>(list));
    }

    static class Cache<T>
    {
        public readonly static ReadOnlyObservableCollection<T> Empty = new(new ObservableCollection<T>());
    }
}