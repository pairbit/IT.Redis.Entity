using IT.Redis.Entity.Formatters;

namespace IT.Redis.Entity.Attributes;

public class GuidHexFormatterAttribute : RedisValueFormatterAttribute<Guid>
{
    private string Format { get; set; }

    public GuidHexFormatterAttribute() : this("N") { }

    public GuidHexFormatterAttribute(string format)
    {
        Format = format;
    }

    public override IRedisValueFormatter<Guid> GetFormatter() =>
        Format == GuidHexFormatter.Default.Format
        ? GuidHexFormatter.Default
        : new GuidHexFormatter { Format = Format };
}