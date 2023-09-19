namespace StackExchange.Redis.Entity.Formatters;

public class GuidFormatter : NullableFormatter<Guid>
{
    public static readonly GuidFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Guid value)
#if NETSTANDARD2_0
        => value = new Guid((byte[]) redisValue!);
#else
        => value = new Guid(((ReadOnlyMemory<byte>)redisValue).Span);
#endif

    public override RedisValue Serialize(in Guid value) => value.ToByteArray();
}