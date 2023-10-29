using System.Reflection;

namespace IT.Redis.Entity.Internal;

internal static class Ex
{
    public static ArgumentException FieldNotInteger(RedisValue field, Exception ex, string? paramName = null)
        => new($"Field '{field}' is not integer", paramName, ex);

    public static ArgumentException InvalidKeyCount(Type entityType, IReadOnlyList<KeyInfo> keys)
        => new(keys.Count > 0 ? keys.Count == 1
            ? $"Entity '{entityType.FullName}' contains one key '{keys[0].Property.Name}'"
            : $"Entity '{entityType.FullName}' contains {keys.Count} keys '{string.Join(", ", keys.Select(x => x.Property.Name))}'"
            : $"Entity '{entityType.FullName}' has no keys");

    public static ArgumentException InvalidKeyType(Type keyType, PropertyInfo key)
        => new($"Type '{keyType.FullName}' is not the type of key '{key.Name}' of entity '{key.DeclaringType!.FullName}'");

    public static ArgumentException Utf8FormatterNotFound(Type type, string? paramName = null) =>
        new($"Utf8Formatter not found for type '{type.FullName}'", paramName);

    public static ArgumentException Utf8FormatterInvalid(Type type, PropertyInfo property, string? paramName = null) =>
        new($"Type '{type.FullName}' is not a utf8 formatter for property '{property.Name}' declared in type '{property.DeclaringType.FullName}'", paramName);

    public static ArgumentException FormatterInvalid(Type type, PropertyInfo property, string? paramName = null) =>
        new($"Type '{type.FullName}' is not a formatter for property '{property.Name}' declared in type '{property.DeclaringType.FullName}'", paramName);

    public static Exception FormatterNotRegistered(Type type) => new RedisValueFormatterException($"Formatter for type '{type.FullName}' not registered");

    public static Exception InvalidLength(Type type, int length) => new RedisValueFormatterException($"{type.FullName} should be {length} bytes long");
}