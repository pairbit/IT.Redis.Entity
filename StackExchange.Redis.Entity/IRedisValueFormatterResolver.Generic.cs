namespace StackExchange.Redis.Entity;

public interface IRedisValueFormatterResolver
{
    IRedisValueFormatter<T>? GetFormatter<T>();
}