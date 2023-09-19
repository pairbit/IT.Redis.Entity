namespace StackExchange.Redis.Entity.Factories;

public class HashSetFactory : EnumerableFactory
{
    public static readonly HashSetFactory Default = new();

    public override IEnumerable<T> Empty<T>() => new HashSet<T>((IEqualityComparer<T>?)null);


    protected override IEnumerable<T> New<T>(int capacity)
#if NETSTANDARD2_0 || NET461
        => new HashSet<T>();
#else
        => new HashSet<T>(capacity, null);
#endif
}