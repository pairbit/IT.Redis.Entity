namespace IT.Redis.Entity.Formatters;

public class GuidFormatter : UnmanagedFormatter<Guid>
{
    public static readonly GuidFormatter Default = new();
}