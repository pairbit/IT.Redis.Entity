namespace StackExchange.Redis.Entity.Serializers;

public class GuidHexSerializer : Serializer<Guid>
{
    public override void Deserialize(in RedisValue redisValue, ref Guid value)
        => value = new Guid((string)redisValue!);

    public override RedisValue Serialize(in Guid value) 
        => value.ToString("n");
}