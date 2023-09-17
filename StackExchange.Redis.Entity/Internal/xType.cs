using System.Reflection;

namespace StackExchange.Redis.Entity.Internal;

internal static class xType
{
    public static bool IsUnmanaged(this Type type)
    {
        if (type.IsPrimitive || type.IsPointer || type.IsEnum) return true;

        if (!type.IsValueType) return false;

        return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                   .All(x => IsUnmanaged(x.FieldType));
    }

    public static Type GetNullableUnderlyingType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;
}