using IT.Redis.Entity.Configurations;

namespace IT.Redis.Entity.Tests;

public class ReadmeTest
{
    private readonly IDatabase _db;

    //Declaring an entity class with mutable keys
    class Document
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
                    _redisKeyBits = 1;
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
                    _redisKeyBits = 2;
                }
            }
        }

        public string? Name { get; set; }
    }

    class Person1
    {
        private readonly int _id;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649
        private byte[]? _redisKey;
#pragma warning restore CS0649
#pragma warning restore IDE0044 // Add readonly modifier

        public byte[]? RedisKey => _redisKey;

        public Person1(int id) => _id = id;

        public int Id => _id;

        public string? Name { get; set; }
    }

    public ReadmeTest()
    {
        var connection = ConnectionMultiplexer.Connect("localhost:6381,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
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

        configBuilder.Entity<Person1>()
                     .HasKeyPrefix("app:persons")
                     .HasKey(x => x.Id);

        var factory = new RedisEntityFactory(configBuilder.Build());

        var rwDoc = factory.NewReaderWriter<Document>();

        var guid = Guid.NewGuid();
        var doc = new Document
        {
            Guid = guid,
            Index = 1,
            Name = "doc1"
        };

        _db.EntitySet(doc, rwDoc);

        doc.Index = 2;
        doc.Name = "doc2";

        _db.EntitySet(doc, rwDoc);

        doc.Index = 3;
        Assert.That(_db.EntityLoad(doc, rwDoc), Is.False);
        Assert.That(doc.Name, Is.EqualTo("doc2"));

        doc.Index = 1;
        Assert.That(_db.EntityLoad(doc, rwDoc), Is.True);
        Assert.That(doc.Name, Is.EqualTo("doc1"));

        var redisKey = rwDoc.KeyBuilder.BuildKey(null, 0, guid, 1);
        Assert.That(redisKey, Is.EqualTo(doc.RedisKey));

        var redisKey2 = KeyBuilder.Default.BuildKey(null, 0, "app:docs", guid, 1);
        Assert.That(redisKey2, Is.EqualTo(redisKey));

        var doc1 = _db.EntityGet(redisKey, rwDoc);
        Assert.That(doc1, Is.Not.Null);
        Assert.That(doc1.Name, Is.EqualTo("doc1"));

        var redisKey3 = rwDoc.KeyBuilder.BuildKey(null, 0, guid, 3);
        var doc3 = _db.EntityGet(redisKey3, rwDoc);
        Assert.That(doc3, Is.Null);

        var readerWriter = factory.NewReaderWriter<Person1>();

        var person = new Person1(12) { Name = "John" };

        _db.EntitySet(person, readerWriter);

        var person2 = new Person1(12);

        Assert.That(_db.EntityLoad(person2, readerWriter), Is.True);

        Assert.That(person.Name, Is.EqualTo(person2.Name));

        redisKey = readerWriter.KeyBuilder.BuildKey(null, 0, 12);

        Assert.That(person.RedisKey, Is.EqualTo(redisKey));

        redisKey2 = KeyBuilder.Default.BuildKey(null, 0, "app:persons", 12);
        
        Assert.That(redisKey2, Is.EqualTo(redisKey));

        _db.KeyDelete(person.RedisKey);
    }
}

