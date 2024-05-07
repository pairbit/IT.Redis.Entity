namespace IT.Redis.Entity;

public class RedisEntityFields<TEntity>
{
    private readonly IReadOnlyDictionary<string, RedisEntityField<TEntity>> _dictionary;

    public RedisEntityField<TEntity>[] EntityFields { get; }

    public RedisValue[] RedisFields { get; }

    public int Count => _dictionary.Count;

    public RedisEntityField<TEntity> this[string propertyName] => _dictionary[propertyName];

    internal RedisEntityFields(IReadOnlyDictionary<string, RedisEntityField<TEntity>> dictionary)
    {
        _dictionary = dictionary;
        EntityFields = dictionary.Values.ToArray();
        RedisFields = dictionary.Values.Select(x => x.Field).ToArray();
    }

    public RedisEntityFields<TEntity> Sub(params string[] propertyNames)
    {
        if (propertyNames == null) throw new ArgumentNullException(nameof(propertyNames));
        if (propertyNames.Length == 0) throw new ArgumentException("is empty", nameof(propertyNames));

        var sub = new Dictionary<string, RedisEntityField<TEntity>>(propertyNames.Length);
        var dic = _dictionary;

        for (int i = 0; i < propertyNames.Length; i++)
        {
            var propertyName = propertyNames[i];

            if (!dic.TryGetValue(propertyName, out var field))
                throw new KeyNotFoundException($"Property name '{propertyName}' not found");

            sub.Add(propertyName, field);
        }

        return new RedisEntityFields<TEntity>(sub);
    }

    public bool ContainsKey(string propertyName) => _dictionary.ContainsKey(propertyName);

    public bool TryGetValue(
        string propertyName,
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]
#endif
        out RedisEntityField<TEntity> value) => _dictionary.TryGetValue(propertyName, out value);
}