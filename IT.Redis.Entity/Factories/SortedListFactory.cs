namespace IT.Redis.Entity.Factories;

public class SortedListFactory : DictionaryFactoryBase
{
    public static readonly SortedListFactory Default = new();

    public override IDictionary<TKey, TValue> Empty<TKey, TValue>() => new SortedList<TKey, TValue>();

    protected override IDictionary<TKey, TValue> New<TKey, TValue>(int capacity) => new SortedList<TKey, TValue>(capacity);
}