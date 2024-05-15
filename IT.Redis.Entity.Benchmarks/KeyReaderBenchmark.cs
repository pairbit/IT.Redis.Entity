using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IT.Redis.Entity.Configurations;
using StackExchange.Redis;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class KeyReaderBenchmark
{
    class Doc_Rebuild_Auto
    {
        private int _id;
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649
        internal byte[]? _redisKey;
#pragma warning restore CS0649
#pragma warning restore IDE0044 // Add readonly modifier

        private byte _redisKeyBits;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    _redisKeyBits |= 1;
                }
            }
        }

        public string? Name { get; set; }
    }

    class Doc_Build_Auto
    {
        internal byte[]? _redisKey;
        private readonly int _id;

        public int Id => _id;

        public string Name { get; }

        public Doc_Build_Auto(int id, string name)
        {
            _id = id;
            Name = name;
        }
    }

    public KeyReaderBenchmark()
    {
        var builder = new RedisEntityConfigurationBuilder();
        builder.Entity<Doc_Rebuild_Auto>()
               .HasKeyPrefix("ra")
               .HasKey(x => x.Id);

        builder.Entity<Doc_Build_Auto>()
               .HasKeyPrefix("ba")
               .HasKey(x => x.Id);

        RedisEntity.Config = builder.Build();
    }

    [Params(1_000)]
    public int Max { get; set; } = 2;

    [Benchmark]
    public RedisKey Rebuild_Auto()
    {
        var re = RedisEntity<Doc_Rebuild_Auto>.Default;
        var doc = new Doc_Rebuild_Auto();
        RedisKey redisKey = default;
        for (int i = 0; i < Max; i++)
        {
            doc.Id = i;
            redisKey = re.ReadKey(doc);
        }
        return redisKey;
    }

    [Benchmark]
    public RedisKey Build_Auto()
    {
        var re = RedisEntity<Doc_Build_Auto>.Default;
        var doc = new Doc_Build_Auto(1, "Ber");
        RedisKey redisKey = default;
        for (int i = 0; i < Max; i++)
        {
            redisKey = re.ReadKey(doc);
        }
        return redisKey;
    }

    public void Validate()
    {
        if (Build_Auto() != "ba:1") throw new System.InvalidOperationException();
        if (Rebuild_Auto() != "ra:1") throw new System.InvalidOperationException();
    }
}