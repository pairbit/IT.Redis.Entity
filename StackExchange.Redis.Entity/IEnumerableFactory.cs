namespace StackExchange.Redis.Entity;

public delegate void EnumerableBuilder<T, TState>(IEnumerable<T> buffer, in TState state);

public interface IEnumerableFactory
{
    IEnumerable<T> Empty<T>();

    IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder);
}