using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class MethodBenchmark
{
    private static readonly Class _c = new();
    private static readonly Interface _i = _c;

    interface Interface
    {
        int InterfaceMethod(int i);
    }

    class Class : Interface
    {
        public readonly Func<int, int> FuncField = i => ++i;

        public Func<int, int> FuncProp { get; } = i => ++i;

        public int Method(int i) => ++i;

        public virtual int VirtualMethod(int i) => ++i;

        public int InterfaceMethod(int i) => ++i;
    }

    public int Max = 1000;

    [Benchmark]
    public int FuncField()
    {
        int res = 0; 
        var obj = _c;
        for (int i = 0; i < Max; i++)
        {
            res = obj.FuncField(i);
        }
        return res;
    }

    [Benchmark]
    public int FuncProp()
    {
        int res = 0;
        var obj = _c;
        for (int i = 0; i < Max; i++)
        {
            res = obj.FuncProp(i);
        }
        return res;
    }

    [Benchmark]
    public int Method()
    {
        int res = 0; 
        var obj = _c;
        for (int i = 0; i < Max; i++)
        {
            res = obj.Method(i);
        }
        return res;
    }

    [Benchmark]
    public int VirtualMethod()
    {
        int res = 0;
        var obj = _c;
        for (int i = 0; i < Max; i++)
        {
            res = obj.VirtualMethod(i);
        }
        return res;
    }

    [Benchmark]
    public int InterfaceMethod()
    {
        int res = 0;
        var obj = _i;
        for (int i = 0; i < Max; i++)
        {
            res = obj.InterfaceMethod(i);
        }
        return res;
    }

    public void Validate()
    {
        var res = FuncField();
        if (res != FuncProp() ||
            res != Method() || 
            res != VirtualMethod() ||
            res != InterfaceMethod()) throw new InvalidOperationException();
    }
}