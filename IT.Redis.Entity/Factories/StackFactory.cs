namespace IT.Redis.Entity.Factories;

public class StackFactory : EnumerableFactory
{
    public static readonly StackFactory Default = new();

    public override IEnumerable<T> Empty<T>() => new Stack<T>();

    protected override IEnumerable<T> New<T>(int capacity) => new Stack<T>(capacity);
}