namespace IT.Redis.Entity;

public class RedisFieldInfo : ICloneable
{
    public object? Formatter { get; set; }

    public object? Utf8Formatter { get; set; }

    public string? FieldName { get; set; }

    public byte? FieldId { get; set; }

    public bool Ignored { get; set; }

    public bool HasKey { get; set; }

    public Delegate? Writer { get; set; }

    public Delegate? Reader { get; set; }

    public RedisFieldInfo Clone() => (RedisFieldInfo)MemberwiseClone();

    object ICloneable.Clone() => MemberwiseClone();
}