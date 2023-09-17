using System.Collections.ObjectModel;

namespace StackExchange.Redis.Entity.Factories;

public class ObservableCollectionFactory : IEnumerableFactory
{
    public static readonly ObservableCollectionFactory Default = new();

    public IEnumerable<T> Empty<T>() => new ObservableCollection<T>();

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return new ObservableCollection<T>();

        var list = new List<T>(capacity);

        builder(list, in state);

        return new ObservableCollection<T>(list);
    }
}