using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using StackExchange.Redis;
using System;
using System.Text;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class RedisBenchmark
{
    private static readonly IDatabase _db;
    private static readonly byte[] _prefix = Encoding.UTF8.GetBytes("prefix");
    private static readonly RedisKey KeyString = "prefix:0";
    private static readonly RedisKey KeyBytes = Encoding.UTF8.GetBytes("prefix:0");
    private const byte Max = 105;

    static RedisBenchmark()
    {
        var connection = ConnectionMultiplexer.Connect("localhost:6381,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
        _db = connection.GetDatabase()!;
    }

    //[Benchmark]
    public void KES()
    {
        if (_db.KeyExists(KeyString)) throw new InvalidOperationException();
    }

    //[Benchmark]
    public void KEB()
    {
        if (_db.KeyExists(KeyBytes)) throw new InvalidOperationException();
    }

    //[Benchmark]
    public void KE_String()
    {
        for (byte i = 0; i < Max; i++)
        {
            if (_db.KeyExists($"prefix:{i}"))
                throw new InvalidOperationException();
        }
    }

    //[Benchmark]
    public void KE_Default()
    {
        var builder = KeyRebuilder.Default;
        byte[]? key = null;
        var prefix = _prefix;

        for (byte i = 0; i < Max; i++)
        {
            key = builder.RebuildKey(key, 2, prefix, i);

            if (_db.KeyExists(key))
                throw new InvalidOperationException();
        }
    }

    //[Benchmark]
    public void KE_Fixed()
    {
        var builder = KeyRebuilder.Fixed;
        byte[]? key = null;
        var prefix = _prefix;

        for (byte i = 0; i < Max; i++)
        {
            key = builder.RebuildKey(key, 2, prefix, i);

            if (_db.KeyExists(key))
                throw new InvalidOperationException();
        }
    }
}