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
    private static readonly IRedisEntityReaderWriter<Document> _rw = RedisEntity<Document>.ReaderWriter;
    private static readonly IRedisEntityReaderWriter<Document> _rwi
        = new RedisEntityReaderWriterIndex<Document>(new RedisEntityConfiguration(RedisValueFormatterRegistry.Default));

    private static readonly Document Data = Document.Data;
    private static readonly HashEntry[] _entries = _rw.GetEntries(Data);

    public Benchmark()
    {

    }

    [Benchmark]
    public HashEntry[] GetEntriesIndex() => _rwi.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries() => _rw.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_Manual() => _rw1.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_Array() => _rw2.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_ArrayExpression() => _rw3.GetEntries(Data);

    [Benchmark]
    public Document? GetEntityIndex() => _rwi.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity() => _rw.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Manual() => _rw1.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Array() => _rw2.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_ArrayExpression() => _rw3.GetEntity(_entries);
}