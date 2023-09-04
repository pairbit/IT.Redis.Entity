namespace StackExchange.Redis.Entity;

public class RedisEntity<T>
{
    public static readonly RedisValue[] Fields;

    public static void GetEntity(HashEntry[] entries, T entity)
    {
        throw new NotImplementedException();
    }

    public static void GetEntity(RedisValue[] values, RedisValue[] fields, T entity)
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