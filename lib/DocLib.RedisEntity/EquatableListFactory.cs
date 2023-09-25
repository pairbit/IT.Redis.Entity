using IT.Redis.Entity;

namespace DocLib.RedisEntity;

public class EquatableListFactory : EnumerableFactory
{
    public static readonly EquatableListFactory Default = new();

    protected override IEnumerable<T> New<T>(int capacity) => new EquatableList<T>(capacity);
}