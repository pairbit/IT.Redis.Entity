using IT.Redis.Entity.Formatters;

namespace IT.Redis.Entity.Internal;

internal class EnumFormatterRegistry<TEnum, TNumber> : EnumFormatter<TEnum, TNumber>
    where TEnum : struct, Enum
    where TNumber : unmanaged
{
    public EnumFormatterRegistry() : base(RedisValueFormatterRegistry.GetFormatter<TNumber>())
    { }
}