namespace IT.Redis.Entity.Factories;

public class ArrayFactory : EnumerableFactory
{
    public static readonly ArrayFactory Default = new();

    public override IEnumerable<T> Empty<T>() => Array.Empty<T>();

    protected override IEnumerable<T> New<T>(int capacity) => new T[capacity];
}