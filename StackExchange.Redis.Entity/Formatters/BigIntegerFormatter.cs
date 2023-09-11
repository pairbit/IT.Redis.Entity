using System.Numerics;

namespace StackExchange.Redis.Entity.Formatters;

public class BigIntegerFormatter : NullableFormatter<BigInteger>
{
    public static readonly BigIntegerFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref BigInteger value)
        => value = new BigInteger(((ReadOnlyMemory<byte>)redisValue).Span);

    public override RedisValue Serialize(in BigInteger value) => value.ToByteArray();
}