﻿using IT.Redis.Entity.Internal;
using IT.Redis.Entity.Utf8Formatters;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfiguration : IRedisEntityConfiguration
{
    private readonly IRedisValueFormatter _formatter;
    private readonly IReadOnlyDictionary<Type, RedisTypeInfo>? _types;
    private readonly IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> _fields;

    public RedisEntityConfiguration(
        IRedisValueFormatter? formatter,
        IReadOnlyDictionary<Type, RedisTypeInfo>? types,
        IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> fields)
    {
        _formatter = formatter ?? RedisValueFormatterNotRegistered.Default;
        _types = types;
        _fields = fields;
    }

    public bool HasAllFieldsNumeric(Type type)
        => _types != null && _types.TryGetValue(type, out var typeInfo) ? typeInfo.HasAllFieldsNumeric : false;

    public string? GetKeyPrefix(Type type)
        => _types != null && _types.TryGetValue(type, out var typeInfo) ? typeInfo.KeyPrefix : null;

    public RedisValue GetField(PropertyInfo property, out bool hasKey)
    {
        hasKey = false;

        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            if (fieldInfo.Ignored) return RedisValue.Null;

            if (fieldInfo.HasKey)
            {
                hasKey = true;
                return RedisValue.Null;
            }

            if (fieldInfo.FieldId != null) return (int)fieldInfo.FieldId.Value;
            if (fieldInfo.FieldName != null) return fieldInfo.FieldName;
        }

        if (property.Name != Compiler.PropRedisKeyName &&
            property.Name != Compiler.PropRedisKeyBitsName &&
            (property.GetMethod?.IsPublic == true ||
            property.SetMethod?.IsPublic == true)) return property.Name;

        return RedisValue.Null;
    }

    public IRedisValueFormatter GetFormatter(PropertyInfo property)
    {
        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            var formatter = fieldInfo.Formatter;
            if (formatter != null) return new RedisValueFormatterProxy(formatter);
        }
        return _formatter;
    }

    public object GetUtf8Formatter(PropertyInfo property)
    {
        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            var utf8Formatter = fieldInfo.Utf8Formatter;
            if (utf8Formatter != null) return utf8Formatter;
        }
        return Utf8FormatterVar.GetFormatter(property.PropertyType) ?? throw Ex.Utf8FormatterNotFound(property.PropertyType);
    }
}