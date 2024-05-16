namespace IT.Redis.Entity.Internal;

internal class RedisTypeInfo : ICloneable
{
    public string? KeyPrefix { get; set; }

    public Delegate? KeyReader { get; set; }

    public IKeyRebuilder? KeyBuilder { get; set; }

    public RedisTypeInfo Clone() => (RedisTypeInfo)MemberwiseClone();

    object ICloneable.Clone() => MemberwiseClone();
}