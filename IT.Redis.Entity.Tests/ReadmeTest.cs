using IT.Redis.Entity.Configurations;

namespace IT.Redis.Entity.Tests;

public class ReadmeTest
{
    private readonly IDatabase _db;

    class Person1
    {
        private readonly int _id;

#pragma warning disable IDE0044 // Add readonly modifier
        private byte[]? _redisKey;
#pragma warning restore IDE0044 // Add readonly modifier

        public byte[]? RedisKey => _redisKey;

        public Person1(int id)
        {
            _id = id;
        }

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

        RedisEntity.Factory = new RedisEntityFactory(configBuilder.Build());

        var person = new Person1(12) { Name = "John" };

        _db.EntitySet(person);
        _db.KeyDelete(person.RedisKey);
    }
}

