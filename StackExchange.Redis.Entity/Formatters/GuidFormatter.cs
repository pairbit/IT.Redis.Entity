namespace StackExchange.Redis.Entity.Formatters;

public class GuidFormatter : NullableFormatter<Guid>
{
    public static readonly GuidFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Guid value)
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        => value = new Guid(((ReadOnlyMemory<byte>)redisValue).Span);
#else
        => value = new Guid((byte[])redisValue!);
#endif

    public override RedisValue Serialize(in Guid value) => value.ToByteArray();
}