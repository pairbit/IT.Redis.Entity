using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using StackExchange.Redis;
using System.Text;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class KeyBuilderBenchmark
{
    private static readonly string _prefixString = "prefix";
    private static readonly byte[] _prefix = Encoding.UTF8.GetBytes(_prefixString);

    [Params(1, 11, 101, 1_001, 10_001, 100_001)]
    public int Max { get; set; }

    [Benchmark]
    public RedisKey String()
    {
        var prefix = _prefixString;

        RedisKey key = $"{prefix}:0";

        for (int i = 1; i < Max; i++)
        {
            key = $"{prefix}:{i}";
        }
        return key;
    }

    [Benchmark]
    public RedisKey Var()
    {
        var builder = KeyBuilder.Default;
        byte[]? key = null;
        var prefix = _prefix;

        for (int i = 1; i < Max; i++)
        {
            key = builder.BuildKey(key, 2, prefix, i);
        }

        return key;
    }

    [Benchmark]
    public RedisKey Fixed()
    {
        var builder = KeyBuilder.Fixed;
        byte[]? key = null;
        var prefix = _prefix;

        for (int i = 1; i < Max; i++)
        {
            key = builder.BuildKey(key, 2, prefix, i);
        }
        return key;
    }
}