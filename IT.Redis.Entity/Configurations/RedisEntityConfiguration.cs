using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfiguration : IRedisEntityConfiguration
{
    private readonly IRedisValueFormatter _formatter;
    private readonly IReadOnlyDictionary<Type, string>? _keyPrefixes;
    private readonly IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> _fields;

    public RedisEntityConfiguration(
        IRedisValueFormatter? formatter,
        IReadOnlyDictionary<Type, string>? keyPrefixes,
        IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> fields)
    {
        _formatter = formatter ?? RedisValueFormatterNotRegistered.Default;
        _keyPrefixes = keyPrefixes;
        _fields = fields;
    }

    public string? GetKeyPrefix(Type type)
        => _keyPrefixes != null && _keyPrefixes.TryGetValue(type, out var keyPrefix) ? keyPrefix : null;

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

            return fieldInfo.Field;
        }

        if (property.GetMethod?.IsPublic == true ||
            property.SetMethod?.IsPublic == true) return property.Name;

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