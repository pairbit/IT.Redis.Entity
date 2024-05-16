using IT.Redis.Entity.Internal;
using IT.Redis.Entity.Utf8Formatters;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfiguration : IRedisEntityConfiguration
{
    private readonly IRedisValueFormatter _formatter;
    private readonly IReadOnlyDictionary<Type, RedisTypeInfo>? _types;
    private readonly IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> _fields;

    internal RedisEntityConfiguration(
        IRedisValueFormatter? formatter,
        IReadOnlyDictionary<Type, RedisTypeInfo>? types,
        IReadOnlyDictionary<PropertyInfo, RedisFieldInfo> fields)
    {
        _formatter = formatter ?? RedisValueFormatterNotRegistered.Default;
        _types = types;
        _fields = fields;
    }

    public string? GetKeyPrefix(Type type)
        => _types != null && _types.TryGetValue(type, out var typeInfo) ? typeInfo.KeyPrefix : null;

    public BindingFlags GetBindingFlags(Type type)
        => BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public IKeyRebuilder? GetKeyBuilder(Type type) 
        => _types != null && _types.TryGetValue(type, out var typeInfo) ? typeInfo.KeyBuilder : null;

    public bool IsKey(PropertyInfo property)
        => _fields.TryGetValue(property, out var fieldInfo) && fieldInfo.HasKey;

    public bool IsIgnore(PropertyInfo property)
        => _fields.TryGetValue(property, out var fieldInfo) && fieldInfo.Ignored;

    public bool TryGetField(PropertyInfo property, out RedisValue field)
    {
        if (_fields.TryGetValue(property, out var fieldInfo))
        {
            if (fieldInfo.FieldId != null)
            {
                field = (int)fieldInfo.FieldId.Value;
                return true;
            }
            if (fieldInfo.FieldName != null)
            {
                field = fieldInfo.FieldName;
                return true;
            }
        }

        if (property.Name != Compiler.PropNameRedisKey &&
            property.Name != Compiler.PropNameRedisKeyBits &&
            (property.GetMethod?.IsPublic == true ||
            property.SetMethod?.IsPublic == true))
        {
            field = property.Name;
            return true;
        }

        field = RedisValue.Null;
        return false;
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
        => _fields.TryGetValue(property, out var fieldInfo)
            ? (RedisValueWriter<TEntity>?)fieldInfo.Writer : null;

    public RedisValueReader<TEntity>? GetReader<TEntity>(PropertyInfo property)
        => _fields.TryGetValue(property, out var fieldInfo)
            ? (RedisValueReader<TEntity>?)fieldInfo.Reader : null;

    public KeyReader<TEntity>? GetKeyReader<TEntity>()
        => _types != null && _types.TryGetValue(typeof(TEntity), out var typeInfo)
            ? (KeyReader<TEntity>?)typeInfo.KeyReader : null;
}