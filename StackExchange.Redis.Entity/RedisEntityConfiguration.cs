using StackExchange.Redis.Entity.Attributes;
using StackExchange.Redis.Entity.Internal;
using System.Reflection;
using System.Runtime.Serialization;

namespace StackExchange.Redis.Entity;

public class RedisEntityConfiguration : IRedisEntityConfiguration
{
    private readonly IRedisValueFormatter _formatter;

    public RedisEntityConfiguration(IRedisValueFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public IRedisValueFormatter GetFormatter(PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<RedisValueFormatterAttribute>(true);

        if (attr != null)
        {
            var formatter = attr.GetFormatterObject();

            if (formatter == null) throw new InvalidOperationException();

            var formatterType = formatter.GetType();

            //if (formatterType.IsGenericType && formatterType.GetGenericTypeDefinition().Equals)

            return new RedisValueFormatterProxy(formatter);
        }

        return _formatter;
    }

    public RedisValue GetField(PropertyInfo property)
    {
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
}