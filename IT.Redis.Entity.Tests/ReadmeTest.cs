using IT.Redis.Entity.Configurations;

namespace IT.Redis.Entity.Tests;

public class ReadmeTest
{
    private readonly IDatabase _db;

    class Person1
    {
        private readonly int _id;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Field 'ReadmeTest.Person1._redisKey' is never assigned to, and will always have its default value null
        private byte[]? _redisKey;
#pragma warning restore CS0649 // Field 'ReadmeTest.Person1._redisKey' is never assigned to, and will always have its default value null
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

        configBuilder.HasKeyPrefix<Person1>("app:persons")
                     .HasKey<Person1, int>(x => x.Id);

        var factory = new RedisEntityFactory(configBuilder.Build());

        var readerWriter = factory.NewReaderWriter<Person1>();

        var person = new Person1(12) { Name = "John" };

        _db.EntitySet(person, readerWriter);

        var person2 = new Person1(12);

        Assert.That(_db.EntityLoad(person2, readerWriter), Is.True);

        Assert.That(person.Name, Is.EqualTo(person2.Name));

        var redisKey = readerWriter.KeyBuilder.BuildKey(null, 0, 12);

        Assert.That(person.RedisKey, Is.EqualTo(redisKey));

        var redisKey2 = KeyBuilder.Default.BuildKey(null, 0, "app:persons", 12);
        
        Assert.That(redisKey2, Is.EqualTo(redisKey));

        _db.KeyDelete(person.RedisKey);
    }
}

