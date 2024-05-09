namespace IT.Redis.Entity;

public class RedisEntityFields<TEntity>
{
    public static readonly RedisEntityFields<TEntity> Empty = new();
    private readonly IReadOnlyDictionary<string, IRedisEntityField<TEntity>> _dictionary;

    public RedisEntityFields<TEntity> ForRead { get; }

    public RedisEntityFields<TEntity> ForWrite { get; }

    public IRedisEntityField<TEntity>[] Array { get; }

    public RedisValue[] ForRedis { get; }

    public int Count => _dictionary.Count;

    public IRedisEntityField<TEntity> this[string propertyName] => _dictionary[propertyName];

    internal RedisEntityFields(IReadOnlyDictionary<string, IRedisEntityField<TEntity>> dictionary)
    {
        if (dictionary.Count == 0) throw new ArgumentException("Empty", nameof(dictionary));
        _dictionary = dictionary;
        var array = dictionary.Values.ToArray();

        Array = array;
        ForRedis = array.Select(x => x.ForRedis).ToArray();

        var read = 0;
        var write = 0;
        foreach (var field in array)
        {
            if (field.CanRead) read++;
            if (field.CanWrite) write++;
        }

        ForRead = array.Length == read ? this :
                  read == 0 ? Empty : Sub(array.Where(x => x.CanRead), read);

        ForWrite = array.Length == write ? this :
                   write == 0 ? Empty : Sub(array.Where(x => x.CanWrite), write);
    }

    private RedisEntityFields()
    {
        _dictionary = new Dictionary<string, IRedisEntityField<TEntity>>(0);
        ForRead = this;
        ForWrite = this;
        Array = [];
        ForRedis = [];
    }

    public RedisEntityFields<TEntity> Sub(params string[] propertyNames)
    {
        if (propertyNames == null) throw new ArgumentNullException(nameof(propertyNames));
        if (propertyNames.Length == 0) throw new ArgumentException("is empty", nameof(propertyNames));

        var sub = new Dictionary<string, IRedisEntityField<TEntity>>(propertyNames.Length);
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
        out IRedisEntityField<TEntity> value) => _dictionary.TryGetValue(propertyName, out value);

    private static RedisEntityFields<TEntity> Sub(IEnumerable<IRedisEntityField<TEntity>> fields, int capacity)
    {
        var sub = new Dictionary<string, IRedisEntityField<TEntity>>(capacity);

        foreach (var field in fields)
        {
            sub.Add(field.Property.Name, field);
        }

        return new RedisEntityFields<TEntity>(sub);
    }

    public override string ToString() => Count.ToString();
}