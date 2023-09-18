using System.IO.Compression;

namespace StackExchange.Redis.Entity;

public class CompressionOptions
{
    public readonly static CompressionOptions Default = new();

    public int StartLength { get; set; } = 1024 * 1024;//1MB

    public CompressionLevel Level { get; set; } = CompressionLevel.Optimal;
}