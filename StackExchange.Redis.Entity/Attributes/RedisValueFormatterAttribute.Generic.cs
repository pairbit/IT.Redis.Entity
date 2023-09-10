namespace StackExchange.Redis.Entity.Attributes;

public abstract class RedisValueFormatterAttribute<T> : RedisValueFormatterAttribute
{
    public abstract IRedisValueFormatter<T> GetFormatter();

    internal override object GetFormatterObject() => GetFormatter();
}