using StackExchange.Redis.Entity.Formatters;
using System.Runtime.CompilerServices;

namespace StackExchange.Redis.Entity;

public class RedisValueFormatterResolver : IRedisValueFormatterResolver
{
    private static readonly Type NullableType = typeof(Nullable<>);
    private static readonly Type UnmanagedFormatterType = typeof(UnmanagedFormatter<>);

    public IRedisValueFormatter<T>? GetFormatter<T>()
    {
        var type = typeof(T);
        var isGenericType = type.IsGenericType;
        var genericType = isGenericType ? type.GetGenericTypeDefinition() : null;
        var isNullable = isGenericType && genericType!.Equals(NullableType);
        var firstGenericType = isGenericType ? type.GetGenericArguments()[0] : null;
        var notNullableType = isNullable ? firstGenericType! : type;

        Type? formatterType = null;

        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            formatterType = UnmanagedFormatterType.MakeGenericType(notNullableType);
        }

        return formatterType == null ? null : Activator.CreateInstance(formatterType) as IRedisValueFormatter<T>;
    }
}