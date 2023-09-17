namespace StackExchange.Redis.Entity.Factories;

public class LinkedListFactory : EnumerableFactory
{
    public static readonly LinkedListFactory Default = new();

    public override IEnumerable<T> Empty<T>() => new LinkedList<T>();

    protected override IEnumerable<T> New<T>(int capacity) => new LinkedList<T>();
}