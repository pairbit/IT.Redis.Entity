namespace StackExchange.Redis.Entity.Internal;

internal static class Ex
{
    public static Exception FormatterNotRegistered(Type type) => new RedisValueFormatterException($"Formatter for type '{type.FullName}' not registered");
}