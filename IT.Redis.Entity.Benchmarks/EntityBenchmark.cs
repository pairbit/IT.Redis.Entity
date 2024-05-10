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
    private static readonly IRedisValueFormatter _formatter = RedisValueFormatterRegistry.Default;
    private static readonly RedisValueDeserializerProxy _dp = new(_formatter);
    private static readonly IRedisEntity<Document> _re = new RedisEntityImpl<Document>(new DataContractAnnotationConfiguration(RedisValueFormatterRegistry.Default));
    private static readonly IRedisEntity<Document> _re_manual = new RedisEntityImpl<Document>(GetConfig());

    private static readonly Document Data = Document.Data;
    private static readonly RedisValue[] Values = _re.Fields.GetValues(Data);

    [Benchmark]
    public HashEntry[] GetEntries() => _re.Fields.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_Manual() => _re_manual.Fields.GetEntries(Data);

    [Benchmark]
    public HashEntry[] GetEntries_Etalon()
    {
        var d = Data;
        var f = _formatter;
        return [
            new(0, f.Serialize(d.Name)),
#if NET6_0_OR_GREATER
            new(1, f.Serialize(d.StartDate)),
            new(2, f.Serialize(d.EndDate)),
#endif
            new(3, f.Serialize(d.Price)),
            new(4, f.Serialize(d.IsDeleted)),
            new(5, f.Serialize(d.Size)),
            new(6, f.Serialize(d.Created)),
            new(7, f.Serialize(d.Modified)),
            new(8, f.Serialize(d.Id)),
        ];
    }

    [Benchmark]
    public RedisValue[] GetValues() => _re.Fields.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues_Manual() => _re_manual.Fields.GetValues(Data);

    [Benchmark]
    public RedisValue[] GetValues_Etalon()
    {
        var d = Data;
        var f = _formatter;
        return [
            f.Serialize(d.Name),
#if NET6_0_OR_GREATER
            f.Serialize(d.StartDate),
            f.Serialize(d.EndDate),
#endif
            f.Serialize(d.Price),
            f.Serialize(d.IsDeleted),
            f.Serialize(d.Size),
            f.Serialize(d.Created),
            f.Serialize(d.Modified),
            f.Serialize(d.Id),
        ];
    }

    [Benchmark]
    public Document? GetEntity() => _re.Fields.GetEntity(Values);

    [Benchmark]
    public Document? GetEntity_Manual() => _re_manual.Fields.GetEntity(Values);

    [Benchmark]
    public Document? GetEntity_Etalon()
    {
        var v = Values;
        var d = _dp;
        return new()
        {
            Name = d.DeserializeNew<string>(v[0])!,
#if NET6_0_OR_GREATER
            StartDate = d.DeserializeNew<DateOnly>(v[1]),
            EndDate = d.DeserializeNew<DateOnly?>(v[2]),
#endif
            Price = d.DeserializeNew<long>(v[3]),
            IsDeleted = d.DeserializeNew<bool>(v[4]),
            Size = d.DeserializeNew<DocumentSize>(v[5]),
            Created = d.DeserializeNew<DateTime>(v[6]),
            Modified = d.DeserializeNew<DateTime?>(v[7]),
            Id = d.DeserializeNew<Guid>(v[8]),
        };
    }

    public void Validate()
    {
        var entries = GetEntries();
        CheckEquals(entries, GetEntries_Manual());
        CheckEquals(entries, GetEntries_Etalon());

        CheckEquals(Values, GetValues());
        CheckEquals(Values, GetValues_Manual());
        CheckEquals(Values, GetValues_Etalon());

        CheckEquals(Data, GetEntity());
        CheckEquals(Data, GetEntity_Manual());
        CheckEquals(Data, GetEntity_Etalon());
    }

    private static void CheckEquals(HashEntry[] e1, HashEntry[] e2)
    {
        if (ReferenceEquals(e1, e2)) throw new InvalidOperationException("ReferenceEquals(e1, e2)");
        if (e1.Length != e2.Length) throw new InvalidOperationException("e1.Length != e2.Length");
        for (int i = 0; i < e1.Length; i++)
        {
            if (!e1[i].Equals(e2[i])) throw new InvalidOperationException("!e1[i].Equals(e2[i])");
        }
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

    private static IRedisEntityConfiguration GetConfig() =>
        new RedisEntityConfigurationBuilder<Document>(RedisValueFormatterRegistry.Default)
        .HasFieldId(x => x.Name, 0)
#if NET6_0_OR_GREATER
        .HasFieldId(x => x.StartDate, 1)
        .HasFieldId(x => x.EndDate, 2)

        .HasReader(x => x.StartDate, (x, s) => s.Serialize(x.StartDate))
        .HasWriter(x => x.StartDate, (x, v, d) => x.StartDate = d.Deserialize(in v, x.StartDate))
        .HasReader(x => x.EndDate, (x, s) => s.Serialize(x.EndDate))
        .HasWriter(x => x.EndDate, (x, v, d) => x.EndDate = d.Deserialize(in v, x.EndDate))
#endif
        .HasFieldId(x => x.Price, 3)
        .HasFieldId(x => x.IsDeleted, 4)
        .HasFieldId(x => x.Size, 5)
        .HasFieldId(x => x.Created, 6)
        .HasFieldId(x => x.Modified, 7)
        .HasFieldId(x => x.Id, 8)

        .HasReader(x => x.Name, (x, s) => s.Serialize(x.Name))
        .HasWriter(x => x.Name, (x, v, d) => x.Name = d.Deserialize(in v, x.Name))

        .HasReader(x => x.Price, (x, s) => s.Serialize(x.Price))
        .HasWriter(x => x.Price, (x, v, d) => x.Price = d.Deserialize(in v, x.Price))

        .HasReader(x => x.IsDeleted, (x, s) => s.Serialize(x.IsDeleted))
        .HasWriter(x => x.IsDeleted, (x, v, d) => x.IsDeleted = d.Deserialize(in v, x.IsDeleted))

        .HasReader(x => x.Size, (x, s) => s.Serialize(x.Size))
        .HasWriter(x => x.Size, (x, v, d) => x.Size = d.Deserialize(in v, x.Size))

        .HasReader(x => x.Created, (x, s) => s.Serialize(x.Created))
        .HasWriter(x => x.Created, (x, v, d) => x.Created = d.Deserialize(in v, x.Created))

        .HasReader(x => x.Modified, (x, s) => s.Serialize(x.Modified))
        .HasWriter(x => x.Modified, (x, v, d) => x.Modified = d.Deserialize(in v, x.Modified))

        .HasReader(x => x.Id, (x, s) => s.Serialize(x.Id))
        .HasWriter(x => x.Id, (x, v, d) => x.Id = d.Deserialize(in v, x.Id))

        .Build(false);
}