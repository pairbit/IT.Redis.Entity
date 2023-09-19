using System.Numerics;

namespace StackExchange.Redis.Entity.Formatters;

public class BigIntegerFormatter : NullableFormatter<BigInteger>
{
    public static readonly BigIntegerFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref BigInteger value)
#if NETSTANDARD2_0
        => value = new BigInteger((byte[])redisValue!);
#else
        => value = new BigInteger(((ReadOnlyMemory<byte>)redisValue).Span);
#endif

    public override RedisValue Serialize(in BigInteger value) => value.ToByteArray();
}