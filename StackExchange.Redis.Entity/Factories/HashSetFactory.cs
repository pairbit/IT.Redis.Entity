namespace StackExchange.Redis.Entity.Factories;

public class HashSetFactory : EnumerableFactory
{
    public static readonly HashSetFactory Default = new();

    public override IEnumerable<T> Empty<T>() => new HashSet<T>((IEqualityComparer<T>?)null);

#if NETSTANDARD2_0
    protected override IEnumerable<T> New<T>(int capacity) => new HashSet<T>();
#else
    protected override IEnumerable<T> New<T>(int capacity) => new HashSet<T>(capacity, null);
#endif
}