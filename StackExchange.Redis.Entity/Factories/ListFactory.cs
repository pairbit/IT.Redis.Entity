namespace StackExchange.Redis.Entity.Factories;

public class ListFactory : EnumerableFactory
{
    public static readonly ListFactory Default = new();

    public override IEnumerable<T> Empty<T>() => new List<T>();

    protected override IEnumerable<T> New<T>(int capacity) => new List<T>(capacity);
}