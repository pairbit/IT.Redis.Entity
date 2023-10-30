#if NETCOREAPP3_1_OR_GREATER

using System.Text.Json;

namespace IT.Redis.Entity.Formatters;

public class JsonFormatter : IRedisValueFormatter
{
    public static readonly JsonFormatter Default = new();

    public void Deserialize<T>(in RedisValue redisValue, ref T? value)
        => value = JsonSerializer.Deserialize<T>(((ReadOnlyMemory<byte>)redisValue).Span);

    public RedisValue Serialize<T>(in T? value)
        => JsonSerializer.SerializeToUtf8Bytes(value);
}

#endif