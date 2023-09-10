namespace StackExchange.Redis.Entity.Formatters;

public class DecimalFormatter : NullableFormatter<Decimal>
{
    public static readonly DecimalFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref Decimal value) => throw new NotImplementedException();

    public override RedisValue Serialize(in Decimal value) => throw new NotImplementedException();
}