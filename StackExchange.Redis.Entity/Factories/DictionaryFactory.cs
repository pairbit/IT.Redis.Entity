namespace StackExchange.Redis.Entity.Factories;

public class DictionaryFactory : DictionaryFactoryBase
{
    public static readonly DictionaryFactory Default = new();

    protected override IDictionary<TKey, TValue> New<TKey, TValue>(int capacity) => new Dictionary<TKey, TValue>(capacity, null);
}