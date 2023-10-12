namespace IT.Redis.Entity.Internal;

internal static class Ex
{
    public static Exception FormatterNotRegistered(Type type) => new RedisValueFormatterException($"Formatter for type '{type.FullName}' not registered");

    public static Exception InvalidLength(Type type, int length) => new RedisValueFormatterException($"{type.FullName} should be {length} bytes long");
}