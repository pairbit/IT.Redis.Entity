using IT.Redis.Entity.Attributes;

namespace DocLib.RedisEntity;

[RedisKeyPrefix("doc")]
public record DocumentWithReadOnlyKeys
{
    private readonly Guid _id;
    private readonly Guid _clientId;
    private readonly string? _name;
    private readonly string? _key4;

#pragma warning disable IDE0044 // Add readonly modifier
    private byte[]? _redisKey;
#pragma warning restore IDE0044 // Add readonly modifier

    public DocumentWithReadOnlyKeys(Guid id, Guid clientId, string? name, string? key4)
    {
        _id = id;
        _clientId = clientId;
        _name = name;
        _key4 = key4;
    }

    [RedisFieldIgnore]
    public byte[]? RedisKey => _redisKey;

    [RedisKey]
    public Guid Id => _id;

    [RedisKey]
    public Guid ClientId => _clientId;

    [RedisKey]
    public string? Name => _name;

    [RedisKey]
    public string? Key4 => _key4;

    //[RedisKey]
    public string? Key5 { get; init; }

    public string? Data1 { get; set; }

    public string? Data2 { get; set; }
}