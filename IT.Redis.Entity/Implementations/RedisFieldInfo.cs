namespace IT.Redis.Entity;

public class RedisFieldInfo
{
    public RedisValue Field { get; set; }

    public object? Formatter { get; set; }

    public object? Utf8Formatter { get; set; }

    public bool Ignored { get; set; }

    public bool HasKey { get; set; }
}