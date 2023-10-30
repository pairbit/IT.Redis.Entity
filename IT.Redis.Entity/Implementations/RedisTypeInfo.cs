namespace IT.Redis.Entity;

public class RedisTypeInfo : ICloneable
{
    public string? KeyPrefix { get; set; }

    public bool HasAllFieldsNumeric { get; set; }

    public RedisTypeInfo Clone() => (RedisTypeInfo)MemberwiseClone();

    object ICloneable.Clone() => MemberwiseClone();
}