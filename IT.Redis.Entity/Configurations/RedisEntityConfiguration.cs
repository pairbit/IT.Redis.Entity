using IT.Redis.Entity.Internal;
using IT.Redis.Entity.Utf8Formatters;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfiguration : IRedisEntityConfiguration
{
    private readonly bool _autoReaderWriter = true;
    private readonly IRedisValueFormatter _formatter;
    private readonly IReadOnlyDictionary<Type, RedisTypeInfo>? _types;
    private readonly IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> _fields;

    internal RedisEntityConfiguration(
        IRedisValueFormatter? formatter,
        IReadOnlyDictionary<Type, RedisTypeInfo>? types,
        IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> fields,
        bool autoReaderWriter)
    {
        _formatter = formatter ?? RedisValueFormatterNotRegistered.Default;
        _types = types;
        _fields = fields;
        _autoReaderWriter = autoReaderWriter;   
    }

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

        if (property.Name != Compiler.PropNameRedisKey &&
            property.Name != Compiler.PropNameRedisKeyBits &&
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
        //TODO: переделать на форматтер по умолчанию
        return Utf8FormatterVar.GetFormatter(property.PropertyType) ?? throw Ex.Utf8FormatterNotFound(property.PropertyType);
    }

    public RedisValueWriter<TEntity>? GetWriter<TEntity>(PropertyInfo property)
    {
        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            var writer = fieldInfo.Writer;
            if (writer != null) return (RedisValueWriter<TEntity>?)writer;
        }
        return _autoReaderWriter ? Compiler.GetWriter<TEntity>(property) : null;
    }

    public RedisValueReader<TEntity>? GetReader<TEntity>(PropertyInfo property)
    {
        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            var reader = fieldInfo.Reader;
            if (reader != null) return (RedisValueReader<TEntity>?)reader;
        }
        return _autoReaderWriter ? Compiler.GetReader<TEntity>(property) : null;
    }
}