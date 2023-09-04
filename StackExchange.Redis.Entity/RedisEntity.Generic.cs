namespace StackExchange.Redis.Entity;

public class RedisEntity<T>
{
    public static readonly RedisValue[] Fields;

    public static void LoadEntity(T entity, HashEntry[] entries)
    {
        throw new NotImplementedException();
    }

    public static void LoadEntity(T entity, RedisValue[] values, RedisValue[] fields)
    {
        throw new NotImplementedException();
    }

    public static HashEntry[] GetEntries(T entity)
    {
        throw new NotImplementedException();
    }

    public static HashEntry[] GetEntries(T entity, RedisValue[] fields)
    {
        throw new NotImplementedException();
    }

    public static RedisValue GetField(string propName)
    {
        throw new NotImplementedException();
    }

    public static RedisValue[] GetFields(params string[] propNames)
    {
        throw new NotImplementedException();
    }
}