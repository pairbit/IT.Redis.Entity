using System.Reflection;

namespace IT.Redis.Entity;

public interface IRedisEntityConfiguration
{
    RedisValue GetField(PropertyInfo property);

    IRedisValueFormatter GetFormatter(PropertyInfo property);
}