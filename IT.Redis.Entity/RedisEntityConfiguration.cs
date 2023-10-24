using IT.Redis.Entity.Attributes;
using IT.Redis.Entity.Formatters.Utf8;
using IT.Redis.Entity.Internal;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace IT.Redis.Entity;

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

    public IRedisValueFormatter GetFormatter(PropertyInfo property)
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

    public RedisValue GetField(PropertyInfo property, out bool hasKey)
    {
        if (property.GetCustomAttribute<RedisKeyAttribute>() != null)
        {
            hasKey = true;
            return RedisValue.Null;
        }

        hasKey = false;

        if (property.GetCustomAttribute<RedisFieldIgnoreAttribute>() != null) return RedisValue.Null;

        var redisField = property.GetCustomAttribute<RedisFieldAttribute>();
        if (redisField != null)
        {
            if (redisField.Id >= 0) return redisField.Id;
            if (redisField.Name != null) return redisField.Name;
        }

        if (property.GetCustomAttribute<IgnoreDataMemberAttribute>() != null) return RedisValue.Null;

        var dataMember = property.GetCustomAttribute<DataMemberAttribute>();
        if (dataMember != null)
        {
            if (dataMember.Order >= 0) return dataMember.Order;
            if (dataMember.IsNameSetExplicitly) return dataMember.Name;
        }

        return property.GetMethod?.IsPublic == true ||
               property.SetMethod?.IsPublic == true ? property.Name : RedisValue.Null;
    }

    public byte[]? GetKeyPrefix(Type type)
    {
        var redisKeyPrefix = type.GetCustomAttribute<RedisKeyPrefixAttribute>();
        return redisKeyPrefix == null ? null : Encoding.UTF8.GetBytes(redisKeyPrefix.Prefix);
    }

    public object GetUtf8Formatter(PropertyInfo property)
    {
        if (property.PropertyType == typeof(Guid)) return GuidUtf8Formatter.Default;
        if (property.PropertyType == typeof(string)) return StringUtf8Formatter.Default;

        throw new NotSupportedException();
    }
}