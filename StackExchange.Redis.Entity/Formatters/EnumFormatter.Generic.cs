using System.Runtime.CompilerServices;

namespace StackExchange.Redis.Entity.Formatters;

public class EnumFormatter<TEnum, T> : NullableFormatter<TEnum>
    where TEnum : struct, Enum
    where T : unmanaged
{
    public EnumFormatter()
    {
        if (Unsafe.SizeOf<TEnum>() != Unsafe.SizeOf<T>())
            throw new InvalidOperationException($"Enum '{typeof(TEnum).FullName}' cannot be converted to '{typeof(T).FullName}'");
    }

    public override void DeserializeNotNull(in RedisValue redisValue, ref TEnum value)
    {
        T underlyingValue = default;

        RedisValueFormatterRegistry.GetFormatter<T>().Deserialize(in redisValue, ref underlyingValue);

        value = Unsafe.As<T, TEnum>(ref Unsafe.AsRef(in underlyingValue));
    }

    public override RedisValue Serialize(in TEnum value)
    {
        var underlyingValue = Unsafe.As<TEnum, T>(ref Unsafe.AsRef(in value));

        return RedisValueFormatterRegistry.GetFormatter<T>().Serialize(in underlyingValue);
    }
}