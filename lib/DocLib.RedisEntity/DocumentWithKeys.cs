using IT.Redis.Entity.Attributes;

namespace DocLib.RedisEntity;

[RedisKeyPrefix("doc")]
public record DocumentWithKeys
{
#pragma warning disable IDE0044 // Add readonly modifier
    private byte[]? _redisKey;
#pragma warning restore IDE0044 // Add readonly modifier

    [RedisFieldIgnore]
    public byte[]? RedisKey => _redisKey;

    [RedisKey]
    public Guid Id { get; set; }

    [RedisKey]
    public Guid ClientId { get; set; }

    [RedisKey]
    public string? Name { get; set; }

    [RedisKey]
    public string? Key4 { get; set; }

    //[RedisKey]
    public string? Key5 { get; set; }

    public string? Data1 { get; set; }

    public string? Data2 { get; set; }

    public void RedisKeyClear() => _redisKey = null;
}