using IT.Redis.Entity.Attributes;
using IT.Redis.Entity.Internal;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class RedisEntityConfiguration : IRedisEntityConfiguration
{
    private readonly IRedisValueFormatter _formatter = new RedisValueFormatterNotRegistered();

    public RedisEntityConfiguration()
    {

    }

    public RedisEntityConfiguration(IRedisValueFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public virtual IRedisValueFormatter GetFormatter(PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<RedisValueFormatterAttribute>(true);

        if (attr != null)
        {
            var formatter = attr.GetFormatterObject() ?? throw new InvalidOperationException();

            var formatterPropertyType = typeof(IRedisValueFormatter<>).MakeGenericType(property.PropertyType);

            if (!formatterPropertyType.IsAssignableFrom(formatter.GetType()))
                throw new InvalidOperationException();

            return new RedisValueFormatterProxy(formatter);
        }

        return _formatter;
    }

    public virtual RedisValue GetField(PropertyInfo property, out bool isKey)
    {
        if (IsKey(property))
        {
            isKey = true;
            return RedisValue.Null;
        }

        isKey = false;

        if (IsIgnore(property)) return RedisValue.Null;

        TryGetField(property, out var field);

        return field;
    }

    public virtual string? GetKeyPrefix(Type type)
        => type.GetCustomAttribute<RedisKeyPrefixAttribute>()?.Prefix;

    public virtual object GetUtf8Formatter(PropertyInfo property)
        => Utf8FormatterVar.GetFormatter(property.PropertyType) ?? throw new NotSupportedException();

    protected virtual bool IsKey(PropertyInfo property) => property.GetCustomAttribute<RedisKeyAttribute>() != null;

    protected virtual bool IsIgnore(PropertyInfo property) => property.GetCustomAttribute<RedisFieldIgnoreAttribute>() != null;

    protected virtual bool TryGetField(PropertyInfo property, out RedisValue field)
    {
        var redisField = property.GetCustomAttribute<RedisFieldAttribute>();
        if (redisField != null)
        {
            if (redisField.Id >= 0)
            {
                field = redisField.Id;
                return true;
            }
            if (redisField.Name != null)
            {
                field = redisField.Name;
                return true;
            }
        }

        if (property.GetMethod?.IsPublic == true ||
            property.SetMethod?.IsPublic == true)
        {
            field = property.Name;
            return true;
        }

        field = RedisValue.Null;
        return false;
    }
}