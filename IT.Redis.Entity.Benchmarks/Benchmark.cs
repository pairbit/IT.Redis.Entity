using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DocLib;
using DocLib.RedisEntity;
using IT.Redis.Entity.Configurations;
using StackExchange.Redis;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class Benchmark
{
    private static readonly IRedisEntityReaderWriter<Document> _rw3 = new RedisDocumentArrayExpression();
    private static readonly IRedisEntityReaderWriter<Document> _rw2 = new RedisDocumentArray();
    private static readonly IRedisEntityReaderWriter<Document> _rw1 = new RedisDocument();
    private static readonly IRedisEntityReaderWriter<Document> _rw
        = new RedisEntityReaderWriter<Document>(new AnnotationConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly IRedisEntityReaderWriter<Document> _rwi
        = new RedisEntityReaderWriterIndex<Document>(new DataContractAnnotationConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly IRedisEntity<Document> _re = new RedisEntityImpl<Document>(new AnnotationConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly Document Data = Document.Data;
    private static readonly HashEntry[] _entries = _rw.GetEntries(Data);

    public Benchmark()
    {

    }

    //[Benchmark]
    public HashEntry[] GetEntries_FAST() => _re.Fields.GetEntries(Data);

    //[Benchmark]
    public HashEntry[] GetEntries_Index() => _rwi.GetEntries(Data);

    //[Benchmark]
    public HashEntry[] GetEntries() => _rw.GetEntries(Data);

    //[Benchmark]
    public HashEntry[] GetEntries_Manual() => _rw1.GetEntries(Data);

    //[Benchmark]
    public HashEntry[] GetEntries_Array() => _rw2.GetEntries(Data);

    //[Benchmark]
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
        var e = GetEntries();
        Equals(e, GetEntries_FAST());
        Equals(e, GetEntries_Index());
        Equals(e, GetEntries_Manual());
        Equals(e, GetEntries_Array());
        Equals(e, GetEntries_ArrayExpression());

        var entity = GetEntity();
    }

    private static bool Equals(HashEntry[] e1, HashEntry[] e2)
    {
        if (e1.Length != e2.Length) return false;
        for (int i = 0; i < e1.Length; i++)
        {
            if (!e1[i].Equals(e2[i])) return false;
        }
        return true;
    }
}