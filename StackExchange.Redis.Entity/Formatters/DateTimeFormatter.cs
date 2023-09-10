namespace StackExchange.Redis.Entity.Formatters;

public class DateTimeFormatter : NullableFormatter<DateTime>
{
    public static readonly DateTimeFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref DateTime value) => value = new DateTime((long)redisValue);

    public override RedisValue Serialize(in DateTime value) => value.Ticks;
}