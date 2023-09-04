namespace StackExchange.Redis.Entity;

public class RedisEntity<T>
{
    public static void GetEntity(HashEntry[] entries, T entity)
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

    public static RedisValue[] GetFields(string propName)
    {
        throw new NotImplementedException();
    }
}