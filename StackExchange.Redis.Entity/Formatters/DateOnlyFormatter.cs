namespace StackExchange.Redis.Entity.Formatters;

public class DateOnlyFormatter : NullableFormatter<DateOnly>
{
    public static readonly DateOnlyFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref DateOnly value) => value = DateOnly.FromDayNumber((int)redisValue);

    public override RedisValue Serialize(in DateOnly value) => value.DayNumber;
}