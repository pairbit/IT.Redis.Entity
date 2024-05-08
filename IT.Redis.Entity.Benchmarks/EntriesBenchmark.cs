using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DocLib;
using DocLib.RedisEntity;
using IT.Redis.Entity.Configurations;
using StackExchange.Redis;
using System;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class EntriesBenchmark
{
    private static readonly IRedisEntityReaderWriter<Document> _rw3 = new RedisDocumentArrayExpression();
    private static readonly IRedisEntityReaderWriter<Document> _rw2 = new RedisDocumentArray();
    private static readonly IRedisEntityReaderWriter<Document> _rw1 = new RedisDocument();
    private static readonly IRedisEntityReaderWriter<Document> _rw
        = new RedisEntityReaderWriter<Document>(new DataContractAnnotationConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly IRedisEntityReaderWriter<Document> _rwi
        = new RedisEntityReaderWriterIndex<Document>(new DataContractAnnotationConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly IRedisEntity<Document> _re = new RedisEntityImpl<Document>(new DataContractAnnotationConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly Document Data = Document.Data;
    private static readonly HashEntry[] _entries = _rw.GetEntries(Data);

    public EntriesBenchmark()
    {

    }

    [Benchmark]
    public HashEntry[] GetEntries_FAST() => _re.Fields.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_Index() => _rwi.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries() => _rw.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_Manual() => _rw1.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_Array() => _rw2.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_ArrayExpression() => _rw3.GetEntries(Data);

    //[Benchmark]
    //public Document? GetEntity_FAST() => _re.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Index() => _rwi.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity() => _rw.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Manual() => _rw1.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Array() => _rw2.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_ArrayExpression() => _rw3.GetEntity(_entries);

    public void Validate()
    {
        var entries = GetEntries();
        CheckEquals(entries, GetEntries_FAST());
        CheckEquals(entries, GetEntries_Index());
        CheckEquals(entries, GetEntries_Manual());
        CheckEquals(entries, GetEntries_Array());
        CheckEquals(entries, GetEntries_ArrayExpression());

        var entity = GetEntity();
        CheckEquals(entity, GetEntity_Index());
        CheckEquals(entity, GetEntity_Manual());
        CheckEquals(entity, GetEntity_Array());
        CheckEquals(entity, GetEntity_ArrayExpression());
    }

    private static void CheckEquals(HashEntry[] e1, HashEntry[] e2)
    {
        if (e1.Length != e2.Length) throw new InvalidOperationException();
        for (int i = 0; i < e1.Length; i++)
        {
            if (!e1[i].Equals(e2[i])) throw new InvalidOperationException();
        }
    }

    private static void CheckEquals(Document? d1, Document? d2)
    {
        if (d1 == null || d2 == null) throw new InvalidOperationException("d1 == null || d2 == null");
        if (ReferenceEquals(d1, d2)) throw new InvalidOperationException("ReferenceEquals(d1, d2)");
        if (!Equals(d1, d2)) throw new InvalidOperationException("!Equals(d1, d2)");
    }
}