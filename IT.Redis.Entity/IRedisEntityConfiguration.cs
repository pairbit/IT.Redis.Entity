using System.Reflection;

namespace IT.Redis.Entity;

public interface IRedisEntityConfiguration
{
    byte[]? GetKeyPrefix(Type type);

    RedisValue GetField(PropertyInfo property, out bool hasKey);

    IRedisValueFormatter GetFormatter(PropertyInfo property);

    object GetFixFormatter(PropertyInfo property);
}