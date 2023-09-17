namespace StackExchange.Redis.Entity;

public interface IEnumerableFactory<TEnumerable, T> where TEnumerable : IEnumerable<T>
{
    TEnumerable Empty();

    TEnumerable New<TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder);
}