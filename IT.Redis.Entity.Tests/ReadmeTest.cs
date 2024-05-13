using IT.Redis.Entity.Configurations;

namespace IT.Redis.Entity.Tests;

public class ReadmeTest
{
    private readonly IDatabase _db;

    //Declaring an entity class with mutable keys
    class Document : IKeyReader
    {
        private Guid _guid;
        private int _index;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649
        private byte[]? _redisKey;
#pragma warning restore CS0649
#pragma warning restore IDE0044 // Add readonly modifier

        private byte _redisKeyBits;

        public byte[]? RedisKey => _redisKey;

        public byte RedisKeyBits => _redisKeyBits;

        public Guid Guid
        {
            get => _guid;
            set
            {
                if (_guid != value)
                {
                    _guid = value;
                    _redisKeyBits |= 1;
                }
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    _redisKeyBits |= 2;
                }
            }
        }

        public string? Name { get; set; }

        internal static byte[] KeyReader(Document entity, IKeyRebuilder builder)
        {
            var redisKey = entity._redisKey;
            var redisKeyBits = entity._redisKeyBits;
            if (redisKeyBits > 0 || redisKey == null)
            {
                entity._redisKey = redisKey = builder.RebuildKey(redisKey, redisKeyBits, entity._guid, entity._index);
                entity._redisKeyBits = 0;
            }
            return redisKey;
        }

        byte[] IKeyReader.ReadKey(IKeyRebuilder builder)
        {
            var redisKey = _redisKey;
            var redisKeyBits = _redisKeyBits;
            if (redisKeyBits > 0 || redisKey == null)
            {
                _redisKey = redisKey = builder.RebuildKey(redisKey, redisKeyBits, _guid, _index);
                _redisKeyBits = 0;
            }
            return redisKey;
        }
    }

    class Person : IKeyReader
    {
        private readonly int _id;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649
        private byte[]? _redisKey;
#pragma warning restore CS0649
#pragma warning restore IDE0044 // Add readonly modifier

        public byte[]? RedisKey => _redisKey;

        public Person(int id) => _id = id;

        public int Id => _id;

        public string? Name { get; set; }

        internal static byte[] KeyReader(Person entity, IKeyRebuilder builder)
        {
            return entity._redisKey ??= builder.BuildKey(entity.Id);
        }

        byte[] IKeyReader.ReadKey(IKeyRebuilder builder)
        {
            return _redisKey ??= builder.BuildKey(_id);
        }
    }

    class Manual : IKeyReader
    {
        private byte[]? _redisKey;

        public byte[]? RedisKey
        {
            get => _redisKey;
            set => _redisKey = value;
        }

        public string? Name { get; set; }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static byte[] KeyReader(Manual entity, IKeyRebuilder builder)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return entity._redisKey!;
        }

        byte[] IKeyReader.ReadKey(IKeyRebuilder builder)
        {
            return _redisKey!;
        }
    }

    public ReadmeTest()
    {
        var connection = ConnectionMultiplexer.Connect(Const.Connection);
        _db = connection.GetDatabase()!;
    }

#if NETCOREAPP3_1_OR_GREATER

    [Test]
    public void JsonFormatterTest() => Test(IT.Redis.Entity.Formatters.JsonFormatter.Default);
#endif

    public void Test(IRedisValueFormatter formatter)
    {
        //Configure factory

        var configBuilder = new RedisEntityConfigurationBuilder(formatter);

        configBuilder.Entity<Document>()
                     .HasKeyPrefix("app:docs")
                     .HasKey(x => x.Guid)
                     .HasKey(x => x.Index);
        //.HasKeyReader(Document.KeyReader);

        configBuilder.Entity<Person>()
                     .HasKeyPrefix("app:persons")
                     .HasKey(x => x.Id);
        //.HasKeyReader(Person.KeyReader);

        configBuilder.Entity<Manual>()
                     .HasKeyPrefix("app:manuals");
                     //.HasKeyReader(Manual.KeyReader);

        var config = configBuilder.Build();

        var reDoc = config.New<Document>();

        var guid = Guid.NewGuid();
        var doc = new Document
        {
            Guid = guid,
            Index = 1,
            Name = "doc1"
        };

        _db.EntitySet(doc, reDoc);

        doc.Index = 2;
        doc.Name = "doc2";

        _db.EntitySet(doc, reDoc);
        Assert.That(_db.KeyDelete(doc.RedisKey), Is.True);

        doc.Index = 3;
        Assert.That(_db.EntityLoad(doc, reDoc), Is.False);
        Assert.That(doc.Name, Is.EqualTo("doc2"));

        doc.Index = 1;
        Assert.That(_db.EntityLoad(doc, reDoc), Is.True);
        Assert.That(doc.Name, Is.EqualTo("doc1"));

        var redisKey = reDoc.KeyBuilder.BuildKey(guid, 1);
        Assert.That(redisKey, Is.EqualTo(doc.RedisKey));

        var redisKey2 = KeyRebuilder.Default.BuildKey("app:docs", guid, 1);
        Assert.That(redisKey2, Is.EqualTo(redisKey));
        
        var doc1 = _db.EntityGet(redisKey, reDoc.Fields);
        Assert.That(doc1, Is.Not.Null);
        Assert.That(doc1.Guid, Is.Default);
        Assert.That(doc1.Name, Is.EqualTo("doc1"));
        Assert.That(_db.KeyDelete(doc.RedisKey), Is.True);

        var redisKey3 = reDoc.KeyBuilder.BuildKey(guid, 3);
        var doc3 = _db.EntityGet(redisKey3, reDoc.Fields);
        Assert.That(doc3, Is.Null);

        var rep = config.New<Person>();

        var person = new Person(12) { Name = "John" };

        _db.EntitySet(person, rep);

        var person2 = new Person(12);

        Assert.That(_db.EntityLoad(person2, rep), Is.True);

        Assert.That(person.Name, Is.EqualTo(person2.Name));

        redisKey = rep.KeyBuilder.BuildKey(12);

        Assert.That(person.RedisKey, Is.EqualTo(redisKey));

        redisKey2 = KeyRebuilder.Default.BuildKey("app:persons", 12);
        
        Assert.That(redisKey2, Is.EqualTo(redisKey));

        Assert.That(_db.KeyDelete(person.RedisKey), Is.True);

        var rem = config.New<Manual>();

        var manual = new Manual { Name = "m1" };

        manual.RedisKey = KeyBuilder.Default.BuildKey("app:manuals", 1);

        _db.EntitySet(manual, rem);

        Assert.That(_db.KeyDelete(manual.RedisKey), Is.True);
    }
}