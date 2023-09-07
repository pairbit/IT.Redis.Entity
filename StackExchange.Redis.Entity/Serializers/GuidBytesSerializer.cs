namespace StackExchange.Redis.Entity.Serializers;

public class GuidBytesSerializer : Serializer<Guid>
{
    public override void Deserialize(in RedisValue redisValue, ref Guid value)
        => value = new Guid(((ReadOnlyMemory<byte>)redisValue).Span);

    public override RedisValue Serialize(in Guid value) 
        => value.ToByteArray();
}