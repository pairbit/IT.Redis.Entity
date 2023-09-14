namespace StackExchange.Redis.Entity;

public interface IEnumerableFactory
{
    TEnumerable New<TEnumerable, T>(int capacity) where TEnumerable : IEnumerable<T>;
}