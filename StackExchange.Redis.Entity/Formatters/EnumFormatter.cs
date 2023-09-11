namespace StackExchange.Redis.Entity.Formatters;

public class EnumFormatter : IRedisValueFormatter<Enum>
{
    public static readonly EnumFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref Enum? value)
        => throw new NotImplementedException("EnumFormatter");

    public RedisValue Serialize(in Enum? value)
        => throw new NotImplementedException("EnumFormatter");
}