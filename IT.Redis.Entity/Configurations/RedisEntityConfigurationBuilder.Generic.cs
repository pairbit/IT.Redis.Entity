using System.Linq.Expressions;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfigurationBuilder<TEntity>
{
    private readonly IRedisValueFormatter? _formatter;
    private readonly IDictionary<Type, RedisTypeInfo> _types;
    private readonly IDictionary<PropertyInfo, RedisFieldInfo> _fields;

    public RedisEntityConfigurationBuilder(IRedisValueFormatter? formatter = null)
    {
        _types = new Dictionary<Type, RedisTypeInfo>(1);
        _fields = new Dictionary<PropertyInfo, RedisFieldInfo>(RedisEntity<TEntity>.Properties.Length);
        _formatter = formatter;
    }

    public RedisEntityConfigurationBuilder(
        IRedisValueFormatter? formatter,
        IDictionary<Type, RedisTypeInfo> types,
        IDictionary<PropertyInfo, RedisFieldInfo> fields)
    {
        _types = types ?? throw new ArgumentNullException(nameof(types));
        _fields = fields ?? throw new ArgumentNullException(nameof(fields));
        _formatter = formatter;
    }

    public RedisEntityConfiguration Build()
    {
        var types = _types.TryGetValue(typeof(TEntity), out var typeInfo)
            ? new Dictionary<Type, RedisTypeInfo>(1) { { typeof(TEntity), typeInfo.Clone() } }
            : null;

        var fields = new Dictionary<PropertyInfo, RedisFieldInfo>(_fields.Count);

        foreach (var item in _fields)
        {
            if (item.Key.DeclaringType == typeof(TEntity))
            {
                fields.Add(item.Key, item.Value.Clone());
            }
        }
        return new RedisEntityConfiguration(_formatter, types, fields);
    }

    public RedisEntityConfigurationBuilder<TEntity> HasKeyPrefix(string keyPrefix)
    {
        if (keyPrefix == null) throw new ArgumentNullException(nameof(keyPrefix));
        if (keyPrefix.Length == 0) throw new ArgumentException("Key prefix is empty", nameof(keyPrefix));

        if (_types.TryGetValue(typeof(TEntity), out var typeInfo))
        {
            typeInfo.KeyPrefix = typeInfo.KeyPrefix == null
                ? keyPrefix : $"{typeInfo.KeyPrefix}:{keyPrefix}";
        }
        else
        {
            _types.Add(typeof(TEntity), new RedisTypeInfo { KeyPrefix = keyPrefix });
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

    public RedisEntityConfigurationBuilder<TEntity> HasFieldId<T>(Expression<Func<TEntity, T>> propertySelector, byte fieldId)
    {
        var property = GetProperty(propertySelector);

        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            fieldInfo.FieldId = fieldId;
        }
        else
        {
            _fields.Add(property, new RedisFieldInfo { FieldId = fieldId });
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
            fieldInfo.FieldName = fieldName;
        }
        else
        {
            _fields.Add(property, new RedisFieldInfo { FieldName = fieldName });
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

    public RedisEntityConfigurationBuilder<TEntity> HasWriter<T>(Expression<Func<TEntity, T>> propertySelector, RedisValueWriter<TEntity> writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));

        GetFieldInfo(GetProperty(propertySelector)).Writer = writer;

        return this;
    }

    public RedisEntityConfigurationBuilder<TEntity> HasReader<T>(Expression<Func<TEntity, T>> propertySelector, RedisValueReader<TEntity> reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));

        GetFieldInfo(GetProperty(propertySelector)).Reader = reader;

        return this;
    }

    private RedisFieldInfo GetFieldInfo(PropertyInfo property)
    {
        if (!_fields.TryGetValue(property, out var fieldInfo))
        {
            fieldInfo = new RedisFieldInfo();
            _fields.Add(property, fieldInfo);
        }

        return fieldInfo;
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