using System.Reflection;

namespace IT.Redis.Entity.Internal;

internal static class xType
{
    static readonly Type NullableType = typeof(Nullable<>);

    public static bool IsNullable(this Type type) => type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == NullableType;

    public static bool IsUnmanaged(this Type type)
    {
        if (type.IsPrimitive || type.IsPointer || type.IsEnum) return true;

        if (!type.IsValueType) return false;

        return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                   .All(x => IsUnmanaged(x.FieldType));
    }

    public static Type GetNullableUnderlyingType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;
}