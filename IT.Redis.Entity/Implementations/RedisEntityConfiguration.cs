using IT.Redis.Entity.Attributes;
using IT.Redis.Entity.Internal;
using IT.Redis.Entity.Utf8Formatters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.Serialization;

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
        if (property.GetCustomAttribute<RedisKeyAttribute>() != null ||
            property.GetCustomAttribute<KeyAttribute>() != null)
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

        if (property.GetCustomAttribute<NotMappedAttribute>() != null) return RedisValue.Null;

        var column = property.GetCustomAttribute<ColumnAttribute>();
        if (column != null)
        {
            if (column.Order >= 0) return column.Order;
            if (column.Name != null) return column.Name;
        }

        return property.GetMethod?.IsPublic == true ||
               property.SetMethod?.IsPublic == true ? property.Name : RedisValue.Null;
    }

    public string? GetKeyPrefix(Type type)
    {
        var redisKeyPrefix = type.GetCustomAttribute<RedisKeyPrefixAttribute>();
        if (redisKeyPrefix != null) return redisKeyPrefix.Prefix;

        var table = type.GetCustomAttribute<TableAttribute>();
        if (table != null) return table.Schema == null ? table.Name : $"{table.Schema}:{table.Name}";

        return null;
    }

    public object GetUtf8Formatter(PropertyInfo property)
    {
        if (property.PropertyType == typeof(Guid)) return GuidUtf8Formatter.Default;
        if (property.PropertyType == typeof(string)) return StringUtf8Formatter.Default;

        throw new NotSupportedException();
    }
}