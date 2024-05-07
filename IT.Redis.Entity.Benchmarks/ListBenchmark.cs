using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Collections.Generic;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class ListBenchmark
{
    private static readonly int[] _array = [8, 7, 6, 1, 8, 7, 6, 1, 8, 7, 6, 1, 8, 7, 6, 1];
    private static readonly LinkedList<int> _ll = new(_array);

    [Benchmark]
    public int LinkedListFor()
    {
        var sum = 0;
        var ll = _ll;
        for (var node = ll.First; node != null; node = node.Next)
        {
            sum += node.Value;
        }
        return sum;
    }

    [Benchmark]
    public int LinkedListForeach()
    {
        var sum = 0;
        foreach (var val in _ll)
        {
            sum += val;
        }
        return sum;
    }

    [Benchmark]
    public int Array()
    {
        var sum = 0;
        var ar = _array;
        for (int i = 0; i < ar.Length; i++)
        {
            sum += ar[i];
        }
        return sum;
    }
}