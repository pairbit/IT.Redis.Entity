using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using StackExchange.Redis;
using System;
using System.Buffers.Text;
using System.Text;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class KeyBuilderBenchmark
{
    private static readonly string _prefixString = "prefix";
    private static readonly byte[] _prefix = Encoding.UTF8.GetBytes(_prefixString);
    private static readonly byte[] _zeroD10 = Encoding.UTF8.GetBytes($"{_prefixString}:{0:D10}");

    [Params(11, 101, 1_001, 10_001, 100_001)]
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
            key = builder.BuildKey(key, 2, in prefix, in i);
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
            key = builder.BuildKey(key, 2, in prefix, in i);
        }
        return key;
    }

    [Benchmark]
    public RedisKey Fixed_Manual()
    {
        byte[] key = _zeroD10;
        var prefix = _prefix;
        var format = new System.Buffers.StandardFormat('d', 10);
        var span = key.AsSpan(prefix.Length + 1);

        for (int i = 1; i < Max; i++)
        {
            Utf8Formatter.TryFormat(i, span, out _, format);
        }
        return key;
    }
}