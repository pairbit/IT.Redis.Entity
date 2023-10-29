using System.Linq.Expressions;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfigurationBuilder<TEntity>
{
    private readonly IRedisValueFormatter? _formatter;
    private readonly IDictionary<Type, string> _keyPrefixes;
    private readonly IDictionary<PropertyInfo, RedisFieldInfo> _fields;

    public RedisEntityConfigurationBuilder(IRedisValueFormatter? formatter = null)
    {
        var capacity = RedisEntity<TEntity>.Properties.Length;

        _keyPrefixes = new Dictionary<Type, string>(capacity);
        _fields = new Dictionary<PropertyInfo, RedisFieldInfo>(capacity);
        _formatter = formatter;
    }

    public RedisEntityConfigurationBuilder(
        IRedisValueFormatter? formatter,
        IDictionary<Type, string> keyPrefixes,
        IDictionary<PropertyInfo, RedisFieldInfo> fields)
    {
        _keyPrefixes = keyPrefixes ?? throw new ArgumentNullException(nameof(keyPrefixes));
        _fields = fields ?? throw new ArgumentNullException(nameof(fields));
        _formatter = formatter;
    }

    public RedisEntityConfiguration Build()
    {
        var keyPrefixes = _keyPrefixes.TryGetValue(typeof(TEntity), out var keyPrefix)
            ? new Dictionary<Type, string>(1) { { typeof(TEntity), keyPrefix } }
            : null;

        var fields = new Dictionary<PropertyInfo, RedisFieldInfo>(_fields.Count);

        foreach (var item in _fields)
        {
            if (item.Key.DeclaringType == typeof(TEntity))
            {
                fields.Add(item.Key, item.Value);
            }
        }

        return new RedisEntityConfiguration(_formatter, keyPrefixes, fields);
    }

    public RedisEntityConfigurationBuilder<TEntity> HasKeyPrefix(string keyPrefix)
    {
        if (keyPrefix == null) throw new ArgumentNullException(nameof(keyPrefix));
        if (keyPrefix.Length == 0) throw new ArgumentException("Key prefix is empty", nameof(keyPrefix));

        if (_keyPrefixes.TryGetValue(typeof(TEntity), out var keyPrefixes))
        {
            _keyPrefixes[typeof(TEntity)] = $"{keyPrefixes}:{keyPrefix}";
        }
        else
        {
            _keyPrefixes.Add(typeof(TEntity), keyPrefix);
        }

        return this;
    }

    public RedisEntityConfigurationBuilder<TEntity> HasKey<T>(Expression<Func<TEntity, T>> propertySelector, IUtf8Formatter<T>? utf8Formatter = null)
    {
        var property = GetProperty(propertySelector);

        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            fieldInfo.HasKey = true;
            fieldInfo.Utf8Formatter = utf8Formatter;
        }
        else
        {
            _fields.Add(property, new RedisFieldInfo { HasKey = true, Utf8Formatter = utf8Formatter });
        }

        return this;
    }

    public RedisEntityConfigurationBuilder<TEntity> HasFieldId<T>(Expression<Func<TEntity, T>> propertySelector, int fieldId)
    {
        if (fieldId < 0) throw new ArgumentOutOfRangeException(nameof(fieldId), fieldId, "field id is negative");

        var property = GetProperty(propertySelector);

        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            fieldInfo.Field = fieldId;
        }
        else
        {
            _fields.Add(property, new RedisFieldInfo { Field = fieldId });
        }

        return this;
    }

    public RedisEntityConfigurationBuilder<TEntity> HasFieldName<T>(Expression<Func<TEntity, T>> propertySelector, string fieldName)
    {
        if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
        if (fieldName.Length == 0) throw new ArgumentException("Field name is empty", nameof(fieldName));

        var property = GetProperty(propertySelector);

        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            fieldInfo.Field = fieldName;
        }
        else
        {
            _fields.Add(property, new RedisFieldInfo { Field = fieldName });
        }

        return this;
    }

    public RedisEntityConfigurationBuilder<TEntity> HasFormatter<T>(Expression<Func<TEntity, T>> propertySelector, IRedisValueFormatter<T> formatter)
    {
        if (formatter == null) throw new ArgumentNullException(nameof(formatter));

        var property = GetProperty(propertySelector);

        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            fieldInfo.Formatter = formatter;
        }
        else
        {
            _fields.Add(property, new RedisFieldInfo { Formatter = formatter });
        }

        return this;
    }

    public RedisEntityConfigurationBuilder<TEntity> Ignore<T>(Expression<Func<TEntity, T>> propertySelector)
    {
        var property = GetProperty(propertySelector);

        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            fieldInfo.Ignored = true;
        }
        else
        {
            _fields.Add(property, new RedisFieldInfo { Ignored = true });
        }

        return this;
    }

    private static PropertyInfo GetProperty(LambdaExpression propertySelector)
    {
        if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));

        if (propertySelector.Body is MemberExpression me)
        {
            if (me.Member is PropertyInfo property)
            {
                if (property.DeclaringType == typeof(TEntity)) return property;

                throw new ArgumentException($"Property '{property.Name}' is not declared for type '{typeof(TEntity).FullName}'", nameof(propertySelector));
            }

            throw new ArgumentException($"Member '{me.Member.Name}' is not a property", nameof(propertySelector));
        }

        throw new ArgumentException("Expression is not of type MemberExpression", nameof(propertySelector));
    }
}