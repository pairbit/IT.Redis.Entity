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
public class ValuesBenchmark
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
    private static readonly RedisValue[] _values = _rw.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues_FAST() => _re.Fields.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues_Index() => _rwi.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues() => _rw.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues_Manual() => _rw1.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues_Array() => _rw2.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues_ArrayExpression() => _rw3.GetValues(Data);

    [Benchmark]
    public Document? GetEntity_FAST() => _re.Fields.GetEntity(_values);

    [Benchmark]
    public Document? GetEntity_Index() => _rwi.GetEntity(_values);

    [Benchmark]
    public Document? GetEntity() => _rw.GetEntity(_values);

    [Benchmark]
    public Document? GetEntity_Manual() => _rw1.GetEntity(_values);

    [Benchmark]
    public Document? GetEntity_Array() => _rw2.GetEntity(_values);

    [Benchmark]
    public Document? GetEntity_ArrayExpression() => _rw3.GetEntity(_values);

    public void Validate()
    {
        var values = GetValues();
        CheckEquals(values, GetValues_FAST());
        CheckEquals(values, GetValues_Index());
        CheckEquals(values, GetValues_Manual());
        CheckEquals(values, GetValues_Array());
        CheckEquals(values, GetValues_ArrayExpression());

        CheckEquals(Data, GetEntity());
        CheckEquals(Data, GetEntity_Index());
        CheckEquals(Data, GetEntity_Manual());
        CheckEquals(Data, GetEntity_Array());
        CheckEquals(Data, GetEntity_ArrayExpression());
        CheckEquals(Data, GetEntity_FAST());
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