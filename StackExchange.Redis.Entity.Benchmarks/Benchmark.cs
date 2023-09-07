using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DocLib;
using DocLib.RedisEntity;

namespace StackExchange.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class Benchmark
{
    private static readonly IRedisEntity<Document> _doc3 = new RedisDocumentArrayExpression();
    private static readonly IRedisEntity<Document> _doc2 = new RedisDocumentArray();
    private static readonly IRedisEntity<Document> _doc1 = new RedisDocument();
    private static readonly HashEntry[] _entries = _doc1.GetEntries(Document.Data);

    public Benchmark()
    {

    }

    [Benchmark]
    public HashEntry[] GetEntries() => _doc1.GetEntries(Document.Data);

    [Benchmark]
    public HashEntry[] GetEntries_Array() => _doc2.GetEntries(Document.Data);

    [Benchmark]
    public HashEntry[] GetEntries_ArrayExpression() => _doc3.GetEntries(Document.Data);

    [Benchmark]
    public Document? GetEntity() => _doc1.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_Array() => _doc2.GetEntity(_entries);

    [Benchmark]
    public Document? GetEntity_ArrayExpression() => _doc3.GetEntity(_entries);
}