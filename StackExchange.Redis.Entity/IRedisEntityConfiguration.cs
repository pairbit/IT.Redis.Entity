using System.Reflection;

namespace StackExchange.Redis.Entity;

public interface IRedisEntityConfiguration
{
    RedisValue GetField(PropertyInfo property);

    IRedisValueFormatter GetFormatter(PropertyInfo property);
}