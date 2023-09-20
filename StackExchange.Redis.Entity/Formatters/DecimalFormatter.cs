namespace StackExchange.Redis.Entity.Formatters;

public class DecimalFormatter : UnmanagedFormatter<Decimal>
{
    public static readonly DecimalFormatter Default = new();
}