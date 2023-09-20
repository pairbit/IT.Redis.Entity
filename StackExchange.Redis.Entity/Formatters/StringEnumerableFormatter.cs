using StackExchange.Redis.Entity.Internal;

namespace StackExchange.Redis.Entity.Formatters;

public class StringEnumerableFormatter<TEnumerable> : IRedisValueFormatter<TEnumerable> where TEnumerable : IEnumerable<string?>
{
    private readonly IEnumerableFactory<TEnumerable, string?> _factory;

    public StringEnumerableFormatter(IEnumerableFactory<TEnumerable, string?> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public StringEnumerableFormatter(EnumerableFactory<TEnumerable, string?> factory)
    {
        _factory = new EnumerableFactoryDelegate<TEnumerable, string?>(factory ?? throw new ArgumentNullException(nameof(factory)));
    }

    public void Deserialize(in RedisValue redisValue, ref TEnumerable? value)
    {
        throw new NotImplementedException();
    }

    public RedisValue Serialize(in TEnumerable? value)
    {
        throw new NotImplementedException();
    }
}