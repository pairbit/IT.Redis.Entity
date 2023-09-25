namespace IT.Redis.Entity.Formatters;

public class GuidHexFormatter : NullableFormatter<Guid>
{
    public static readonly GuidHexFormatter Default = new();

    public string Format { get; set; } = "N";

    public override void DeserializeNotNull(in RedisValue redisValue, ref Guid value) => value = new Guid((string)redisValue!);

    public override RedisValue Serialize(in Guid value) => value.ToString(Format);
}