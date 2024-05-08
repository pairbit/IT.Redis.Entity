using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DocLib;
using IT.Redis.Entity.Configurations;
using IT.Redis.Entity.Extensions;
using StackExchange.Redis;
using System;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class EntityBenchmark
{
    private static readonly IRedisEntity<Document> _re = new RedisEntityImpl<Document>(new DataContractAnnotationConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly Document Data = Document.Data;
    private static readonly RedisValue[] Values = _re.Fields.GetValues(Data);

    [Benchmark]
    public HashEntry[] GetEntries() => _re.Fields.GetEntries(Data);

    [Benchmark]
    public RedisValue[] GetValues() => _re.Fields.GetValues(Data);

    [Benchmark]
    public Document? GetEntity() => _re.Fields.GetEntity(Values);

    public void Validate()
    {
        CheckEquals(Values, GetValues());
        
        CheckEquals(Data, GetEntity());
    }

    private static void CheckEquals(RedisValue[] e1, RedisValue[] e2)
    {
        if (ReferenceEquals(e1, e2)) throw new InvalidOperationException("ReferenceEquals(e1, e2)");
        if (e1.Length != e2.Length) throw new InvalidOperationException("e1.Length != e2.Length");
        for (int i = 0; i < e1.Length; i++)
        {
            if (!e1[i].Equals(e2[i])) throw new InvalidOperationException("!e1[i].Equals(e2[i])");
        }
    }

    private static void CheckEquals(Document? d1, Document? d2)
    {
        if (d1 == null || d2 == null) throw new InvalidOperationException("d1 == null || d2 == null");
        if (ReferenceEquals(d1, d2)) throw new InvalidOperationException("ReferenceEquals(d1, d2)");
        if (!Equals(d1, d2)) throw new InvalidOperationException("!Equals(d1, d2)");
    }
}