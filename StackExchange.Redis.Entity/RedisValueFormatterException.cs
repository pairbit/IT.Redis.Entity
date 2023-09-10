namespace StackExchange.Redis.Entity;

public class RedisValueFormatterException : Exception
{
    public RedisValueFormatterException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}