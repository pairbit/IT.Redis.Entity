using ExternalLib;

namespace StackExchange.Redis.Entity.Tests;

public class RedisEntityTest
{
    private IDatabase _db;

    [SetUp]
    public void Setup()
    {
        var connection = ConnectionMultiplexer.Connect(GetConnectionString(6381));
        _db = connection.GetDatabase()!;
    }

    [Test]
    public void HashSetGet()
    {
        _db.HashSet(Doc.Key1, RedisEntity<Document>.GetEntries(Doc.Data1));

        var doc2 = new Document();

        RedisEntity<Document>.GetEntity(_db.HashGetAll(Doc.Key1), doc2);

        Assert.That(doc2, Is.EqualTo(Doc.Data1));

        doc2.EndDate = new DateOnly(2022, 03, 20);
        doc2.IsDeleted = true;

        _db.HashSet(Doc.Key1, RedisEntity<Document>.GetEntries(doc2, new[] { Doc.Fields.IsDeleted }));

        Assert.IsTrue(_db.KeyDelete(Doc.Key1));
    }

    [Test]
    public void EntitySetGet()
    {
        _db.EntitySet(Doc.Key1, Doc.Data1);

        var doc2 = new Document();

        _db.EntityGetAll(Doc.Key1, doc2);

        Assert.That(doc2, Is.EqualTo(Doc.Data1));

        doc2.EndDate = new DateOnly(2022, 03, 20);
        doc2.IsDeleted = true;

        _db.EntitySet(Doc.Key1, doc2, new[] { Doc.Fields.IsDeleted });
        
        Assert.IsTrue(_db.KeyDelete(Doc.Key1));
    }

    static string GetConnectionString(int port = 6379, int db = 0) => $"localhost:{port},defaultDatabase={db},syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False";
}