namespace IT.Redis.Entity.Factories;

public class SortedDictionaryFactory : DictionaryFactoryBase
{
    public static readonly SortedDictionaryFactory Default = new();

    protected override IDictionary<TKey, TValue> New<TKey, TValue>(int capacity) => new SortedDictionary<TKey, TValue>();
}