using System.Runtime.CompilerServices;

namespace IT.Redis.Entity.Formatters;

public class EnumFormatter<TEnum, TNumber> : NullableFormatter<TEnum>
    where TEnum : struct, Enum
    where TNumber : unmanaged
{
    private readonly IRedisValueFormatter<TNumber> _numberFormatter;

    static EnumFormatter()
    {
        if (typeof(TNumber) != typeof(TEnum).GetEnumUnderlyingType())
            throw new ArgumentException($"UnderlyingType enum '{typeof(TEnum).FullName}' is '{typeof(TEnum).GetEnumUnderlyingType().FullName}'", nameof(TNumber));
    }

    public EnumFormatter(IRedisValueFormatter<TNumber> numberFormatter)
    {
        _numberFormatter = numberFormatter ?? throw new ArgumentNullException(nameof(numberFormatter));
    }

    public override void DeserializeNotNull(in RedisValue redisValue, ref TEnum value)
    {
        TNumber numberValue = default;

        _numberFormatter.Deserialize(in redisValue, ref numberValue);

        value = Unsafe.As<TNumber, TEnum>(ref numberValue);
    }

    public override RedisValue Serialize(in TEnum value)
    {
        var numberValue = Unsafe.As<TEnum, TNumber>(ref Unsafe.AsRef(in value));

        return _numberFormatter.Serialize(in numberValue);
    }
}