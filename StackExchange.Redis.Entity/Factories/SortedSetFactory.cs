namespace StackExchange.Redis.Entity.Factories;

public class SortedSetFactory : EnumerableFactory
{
    public static readonly SortedSetFactory Default = new();

    public override IEnumerable<T> Empty<T>() => new SortedSet<T>();

    protected override IEnumerable<T> New<T>(int capacity) => new SortedSet<T>();
}