using IT.Redis.Entity.Configurations;
using System.Text.Json;

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

        public byte[] RedisKey => _redisKey;

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


    class JsonFormatter : IRedisValueFormatter
    {
        public void Deserialize<T>(in RedisValue redisValue, ref T? value)
        {
            var str = (string?)redisValue;
            value = str == null ? default : JsonSerializer.Deserialize<T>(str);
        }

        public RedisValue Serialize<T>(in T? value)
        {
            return JsonSerializer.Serialize(value);
        }
    }

    [Test]
    public void JsonFormatterTest() => Test(new JsonFormatter());

    public void Test(IRedisValueFormatter formatter)
    {
        //Configure factory

        var configBuilder = new RedisEntityConfigurationBuilder(formatter);

        configBuilder.HasKeyPrefix<Person1>("app:persons")
                     .HasKey<Person1, int>(x => x.Id);

        RedisEntity.Factory = new RedisEntityFactory(configBuilder.Build());

        var person = new Person1(12);

        _db.EntitySet(person);
        _db.KeyDelete(person.RedisKey);
    }
}

