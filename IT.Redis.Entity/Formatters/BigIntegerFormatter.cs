using System.Numerics;

namespace IT.Redis.Entity.Formatters;

public class BigIntegerFormatter : NullableFormatter<BigInteger>
{
    public static readonly BigIntegerFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref BigInteger value)
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        => value = new BigInteger(((ReadOnlyMemory<byte>)redisValue).Span);
#else
        => value = new BigInteger((byte[])redisValue!);
#endif

    public override RedisValue Serialize(in BigInteger value) => value.ToByteArray();
}