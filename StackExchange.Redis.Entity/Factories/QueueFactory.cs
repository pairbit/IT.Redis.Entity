namespace StackExchange.Redis.Entity.Factories;

public class QueueFactory : EnumerableFactory
{
    public static readonly QueueFactory Default = new();

    public override IEnumerable<T> Empty<T>() => new Queue<T>();

    protected override IEnumerable<T> New<T>(int capacity) => new Queue<T>(capacity);
}