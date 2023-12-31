﻿using IT.Redis.Entity.Internal;
using System.Linq.Expressions;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfigurationBuilder
{
    private readonly IRedisValueFormatter? _formatter;
    private readonly Dictionary<Type, RedisTypeInfo> _types = new();
    private readonly Dictionary<PropertyInfo, RedisFieldInfo> _fields = new();

    public RedisEntityConfigurationBuilder(IRedisValueFormatter? formatter = null)
    {
        _formatter = formatter;
    }

    public RedisEntityConfiguration Build()
    {
        var types = new Dictionary<Type, RedisTypeInfo>(_types.Count);
        var fields = new Dictionary<PropertyInfo, RedisFieldInfo>(_fields.Count);
        foreach (var item in _types)
        {
            types.Add(item.Key, item.Value.Clone());
        }
        foreach (var item in _fields)
        {
            fields.Add(item.Key, item.Value.Clone());
        }
        return new(_formatter, types, fields);
    }

    public RedisEntityConfigurationBuilder<TEntity> Entity<TEntity>()
        => new(_formatter, _types, _fields);

    #region NonGeneric

    public RedisEntityConfigurationBuilder HasAllFieldsNumeric(Type entityType)
    {
        if (entityType == null) throw new ArgumentNullException(nameof(entityType));

        if (_types.TryGetValue(entityType, out var typeInfo))
        {
            typeInfo.HasAllFieldsNumeric = true;
        }
        else
        {
            _types.Add(entityType, new RedisTypeInfo { HasAllFieldsNumeric = true });
        }

        return this;
    }

    public RedisEntityConfigurationBuilder HasKeyPrefix(Type entityType, string keyPrefix)
    {
        if (entityType == null) throw new ArgumentNullException(nameof(entityType));
        if (keyPrefix == null) throw new ArgumentNullException(nameof(keyPrefix));
        if (keyPrefix.Length == 0) throw new ArgumentException("Key prefix is empty", nameof(keyPrefix));

        if (_types.TryGetValue(entityType, out var typeInfo))
        {
            typeInfo.KeyPrefix = typeInfo.KeyPrefix == null
                ? keyPrefix : $"{typeInfo.KeyPrefix}:{keyPrefix}";
        }
        else
        {
            _types.Add(entityType, new RedisTypeInfo { KeyPrefix = keyPrefix });
        }

        return this;
    }

    public RedisEntityConfigurationBuilder HasKey(PropertyInfo property, object? utf8Formatter = null)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

        if (utf8Formatter != null && !typeof(IUtf8Formatter<>).MakeGenericType(property.PropertyType).IsAssignableFrom(utf8Formatter.GetType()))
            throw Ex.Utf8FormatterInvalid(utf8Formatter.GetType(), property, nameof(utf8Formatter));

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

    public RedisEntityConfigurationBuilder HasFieldId(PropertyInfo property, byte fieldId)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

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

    public RedisEntityConfigurationBuilder HasFieldName(PropertyInfo property, string fieldName)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
        if (fieldName.Length == 0) throw new ArgumentException("Field name is empty", nameof(fieldName));

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

    public RedisEntityConfigurationBuilder HasFormatter(PropertyInfo property, object formatter)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (formatter == null) throw new ArgumentNullException(nameof(formatter));

        if (!typeof(IRedisValueFormatter<>).MakeGenericType(property.PropertyType).IsAssignableFrom(formatter.GetType()))
            throw Ex.FormatterInvalid(formatter.GetType(), property, nameof(formatter));

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

    public RedisEntityConfigurationBuilder Ignore(PropertyInfo property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

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

    #endregion NonGeneric

    #region Generic

    public RedisEntityConfigurationBuilder HasAllFieldsNumeric<TEntity>()
        => HasAllFieldsNumeric(typeof(TEntity));

    public RedisEntityConfigurationBuilder HasKeyPrefix<TEntity>(string keyPrefix)
        => HasKeyPrefix(typeof(TEntity), keyPrefix);

    public RedisEntityConfigurationBuilder HasKey<TEntity, T>(Expression<Func<TEntity, T>> propertySelector, IUtf8Formatter<T>? utf8Formatter = null)
        => HasKey(GetProperty(propertySelector), utf8Formatter);

    public RedisEntityConfigurationBuilder HasFieldId<TEntity, T>(Expression<Func<TEntity, T>> propertySelector, byte fieldId)
        => HasFieldId(GetProperty(propertySelector), fieldId);

    public RedisEntityConfigurationBuilder HasFieldName<TEntity, T>(Expression<Func<TEntity, T>> propertySelector, string fieldName)
        => HasFieldName(GetProperty(propertySelector), fieldName);

    public RedisEntityConfigurationBuilder HasFormatter<TEntity, T>(Expression<Func<TEntity, T>> propertySelector, IRedisValueFormatter<T> formatter)
        => HasFormatter(GetProperty(propertySelector), formatter);

    public RedisEntityConfigurationBuilder Ignore<TEntity, T>(Expression<Func<TEntity, T>> propertySelector)
        => Ignore(GetProperty(propertySelector));

    #endregion Generic

    private static PropertyInfo GetProperty(LambdaExpression propertySelector)
    {
        if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));

        if (propertySelector.Body is MemberExpression me)
        {
            if (me.Member is PropertyInfo property) return property;

            throw new ArgumentException($"Member '{me.Member.Name}' is not a property", nameof(propertySelector));
        }

        throw new ArgumentException("Expression is not of type MemberExpression", nameof(propertySelector));
    }
}