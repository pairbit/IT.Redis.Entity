using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IT.Redis.Entity.Configurations;
using StackExchange.Redis;
using System;

namespace IT.Redis.Entity.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class KeyReaderBenchmark
{
    class Doc_Rebuild_Auto
    {
        private int _id;
        internal byte[]? _redisKey;
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

    class Doc_Rebuild_Interface : IKeyReader
    {
        private int _id;
        internal byte[]? _redisKey;
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

        byte[] IKeyReader.ReadKey(IKeyRebuilder builder)
        {
            var redisKey = _redisKey;
            var redisKeyBits = _redisKeyBits;
            if (redisKeyBits > 0 || redisKey == null)
            {
                _redisKey = redisKey = builder.RebuildKey(redisKey, redisKeyBits, _id);
                _redisKeyBits = 0;
            }
            return redisKey;
        }
    }

    class Doc_Rebuild_Static
    {
        private int _id;
        internal byte[]? _redisKey;
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

        internal static byte[] KeyReader(Doc_Rebuild_Static doc, IKeyRebuilder builder)
        {
            var redisKey = doc._redisKey;
            var redisKeyBits = doc._redisKeyBits;
            if (redisKeyBits > 0 || redisKey == null)
            {
                doc._redisKey = redisKey = builder.RebuildKey(redisKey, redisKeyBits, doc._id);
                doc._redisKeyBits = 0;
            }
            return redisKey;
        }
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

    class Doc_Build_Interface : IKeyReader
    {
        internal byte[]? _redisKey;
        private readonly int _id;

        public int Id => _id;

        public string Name { get; }

        public Doc_Build_Interface(int id, string name)
        {
            _id = id;
            Name = name;
        }

        byte[] IKeyReader.ReadKey(IKeyRebuilder builder)
        {
            return _redisKey ??= builder.BuildKey(_id);
        }
    }

    class Doc_Build_Static
    {
        internal byte[]? _redisKey;
        private readonly int _id;

        public int Id => _id;

        public string Name { get; }

        public Doc_Build_Static(int id, string name)
        {
            _id = id;
            Name = name;
        }

        internal static byte[] KeyReader(Doc_Build_Static doc, IKeyRebuilder builder)
        {
            return doc._redisKey ??= builder.BuildKey(doc._id);
        }
    }

    public KeyReaderBenchmark()
    {
        var builder = new RedisEntityConfigurationBuilder();
        builder.Entity<Doc_Rebuild_Auto>()
               .HasKeyPrefix("ra")
               .HasKey(x => x.Id);

        builder.Entity<Doc_Rebuild_Interface>()
               .HasKeyPrefix("ri")
               .HasKey(x => x.Id);

        builder.Entity<Doc_Rebuild_Static>()
               .HasKeyPrefix("rs")
               .HasKeyReader(Doc_Rebuild_Static.KeyReader)
               .HasKey(x => x.Id);

        builder.Entity<Doc_Build_Auto>()
               .HasKeyPrefix("ba")
               .HasKey(x => x.Id);

        builder.Entity<Doc_Build_Interface>()
               .HasKeyPrefix("bi")
               .HasKey(x => x.Id);

        builder.Entity<Doc_Build_Static>()
               .HasKeyPrefix("bs")
               .HasKeyReader(Doc_Build_Static.KeyReader)
               .HasKey(x => x.Id);

        RedisEntity.Config = builder.Build();
    }

    [Params(100)]
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
    public RedisKey Rebuild_Interface()
    {
        var re = RedisEntity<Doc_Rebuild_Interface>.Default;
        var doc = new Doc_Rebuild_Interface();
        RedisKey redisKey = default;
        for (int i = 0; i < Max; i++)
        {
            doc.Id = i;
            redisKey = re.ReadKey(doc);
        }
        return redisKey;
    }

    [Benchmark]
    public RedisKey Rebuild_Static()
    {
        var re = RedisEntity<Doc_Rebuild_Static>.Default;
        var doc = new Doc_Rebuild_Static();
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

    [Benchmark]
    public RedisKey Build_Interface()
    {
        var re = RedisEntity<Doc_Build_Interface>.Default;
        var doc = new Doc_Build_Interface(1, "Ber");
        RedisKey redisKey = default;
        for (int i = 0; i < Max; i++)
        {
            redisKey = re.ReadKey(doc);
        }
        return redisKey;
    }

    [Benchmark]
    public RedisKey Build_Static()
    {
        var re = RedisEntity<Doc_Build_Static>.Default;
        var doc = new Doc_Build_Static(1, "Ber");
        RedisKey redisKey = default;
        for (int i = 0; i < Max; i++)
        {
            redisKey = re.ReadKey(doc);
        }
        return redisKey;
    }

    public void Validate()
    {
        if (Build_Auto() != "ba:1") throw new InvalidOperationException();
        if (Build_Interface() != "bi:1") throw new InvalidOperationException();
        if (Build_Static() != "bs:1") throw new InvalidOperationException();
        if (Rebuild_Auto() != "ra:1") throw new InvalidOperationException();
        if (Rebuild_Interface() != "ri:1") throw new InvalidOperationException();
        if (Rebuild_Static() != "rs:1") throw new InvalidOperationException();
    }
}