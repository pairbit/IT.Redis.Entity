using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace StackExchange.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class Benchmark
{

    public Benchmark()
    {

    }

    //[Benchmark]
    //public byte[] Json_Serialize() => _jsonSerializer.Serialize(_person);

}