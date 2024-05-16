using DocLib;
using IT.Redis.Entity.Configurations;

namespace IT.Redis.Entity.Tests;

public class RedisEntityTest
{
    private readonly IDatabase _db;
    private readonly RedisEntity<Document> _re;

    private static readonly RedisKey KeyPrefix = "doc:";
    private static readonly RedisKey Key = KeyPrefix.Append("1");
    private static readonly RedisKey NFKey = KeyPrefix.Append("notfound");

    static RedisEntityTest()
    {
        RedisEntity.Config = new AnnotationConfiguration(RedisValueFormatterRegistry.Default);
    }

    public RedisEntityTest() : this(RedisEntity<Document>.Default)
    {

    }

    public RedisEntityTest(RedisEntity<Document> re)
    {
        var connection = ConnectionMultiplexer.Connect(Const.Connection);
        _db = connection.GetDatabase()!;
        _re = re;
    }

    [Test]
    public void HashSet_Multi()
    {
        var re = _re;
        var fields = re.Fields;
        var redisFields = fields.ForRedis;
        var entries = new HashEntry[fields.Count];
        var document = new Document();
        var keys = new RedisKey[100];

        try
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Document.New(document, i);

                fields.ReadEntries(entries, document);

                var key = KeyPrefix.Append(i.ToString());

                _db.HashSet(key, entries);

                Assert.That(fields.GetEntity(_db.HashGet(key, redisFields)), Is.EqualTo(document));

                keys[i] = key;
            }
        }
        finally
        {
            _db.KeyDelete(keys);
        }
    }

    [Test]
    public void HashGet_Multi()
    {
        var re = _re;
        var fields = re.Fields;
        var redisFields = fields.ForRedis;
        try
        {
            _db.EntitySet(Key, Document.Data, re);

            var documents = new Document?[10];

            var values = _db.HashGet(Key, redisFields);

            for (int i = 0; i < documents.Length; i++)
            {
                documents[i] = fields.GetEntity(values);
            }

            var first = documents[0];

            for (int i = 1; i < documents.Length; i++)
            {
                Assert.That(first, Is.EqualTo(documents[i]));
            }
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void HashSetGet()
    {
        var re = _re;
        var fields = re.Fields;
        var EndDate_Modified = fields.Sub(
#if NET6_0_OR_GREATER
            nameof(Document.EndDate),
#endif
            nameof(Document.Modified));

        var Field_IsDeleted = fields[nameof(Document.IsDeleted)];

        Assert.That(fields.GetEntity(_db.HashGet(Key, fields.ForRedis)), Is.Null);
        Assert.That(EndDate_Modified.GetEntity(_db.HashGet(Key, EndDate_Modified.ForRedis)), Is.Null);

        var doc2 = new Document();

        Assert.That(EndDate_Modified.Write(doc2, _db.HashGet(Key, EndDate_Modified.ForRedis)), Is.False);
        Assert.That(Field_IsDeleted.Write(doc2, _db.HashGet(Key, Field_IsDeleted.ForRedis)), Is.False);

        Assert.That(doc2, Is.EqualTo(Document.Empty));

        try
        {
            _db.HashSet(Key, fields.GetEntries(Document.Data));

            Assert.That(fields.Write(doc2, _db.HashGet(Key, fields.ForRedis)), Is.True);

            Assert.That(doc2, Is.EqualTo(Document.Data));
            Assert.That(fields.GetEntity(_db.HashGet(Key, fields.ForRedis)), Is.EqualTo(Document.Data));
#if NET6_0_OR_GREATER
            doc2.EndDate = new DateOnly(2022, 03, 20);
#endif
            doc2.Modified = DateTime.UtcNow;

            _db.HashSet(Key, EndDate_Modified.GetEntries(doc2));

            var doc3 = new Document();

            Assert.That(EndDate_Modified.Write(doc3, _db.HashGet(Key, EndDate_Modified.ForRedis)), Is.True);

            Assert.That(doc2, Is.Not.EqualTo(doc3));
#if NET6_0_OR_GREATER
            Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
#endif
            Assert.That(doc2.Modified, Is.EqualTo(doc3.Modified));

            Assert.That(fields.Write(doc3, _db.HashGet(Key, fields.ForRedis)), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            doc2.IsDeleted = true;

            Assert.That(doc2, Is.Not.EqualTo(doc3));

            Assert.That(_db.HashSet(Key, Field_IsDeleted.ForRedis, Field_IsDeleted.Read(doc2)), Is.False);

            Assert.That(Field_IsDeleted.Write(doc3, _db.HashGet(Key, Field_IsDeleted.ForRedis)), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            Assert.That(Field_IsDeleted.GetEntity(_db.HashGet(Key, Field_IsDeleted.ForRedis)), Is.EqualTo(Document.Deleted));

        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void EntitySetGet()
    {
        var re = _re;
        var EndDate_Modified = re.Sub(
#if NET6_0_OR_GREATER
            nameof(Document.EndDate),
#endif
            nameof(Document.Modified));

        var Field_IsDeleted = re.Fields[nameof(Document.IsDeleted)];

        Assert.That(_db.EntityGet<Document>(Key, Field_IsDeleted), Is.Null);
        Assert.That(_db.EntityGet<Document>(Key, EndDate_Modified), Is.Null);
        Assert.That(_db.EntityGet<Document>(Key), Is.Null);

        var doc2 = new Document();

        Assert.That(_db.EntityLoad(Key, doc2, Field_IsDeleted), Is.False);
        Assert.That(_db.EntityLoad(Key, doc2, EndDate_Modified), Is.False);
        Assert.That(_db.EntityLoad(Key, doc2), Is.False);

        Assert.That(doc2, Is.EqualTo(Document.Empty));

        try
        {
            _db.EntitySet(Key, Document.Data, re);

            Assert.That(_db.EntityLoad(Key, doc2, re), Is.True);

            Assert.That(doc2, Is.EqualTo(Document.Data));
            Assert.That(_db.EntityGet(Key, re), Is.EqualTo(Document.Data));
#if NET6_0_OR_GREATER
            doc2.EndDate = new DateOnly(2022, 03, 20);
#endif
            doc2.Modified = DateTime.UtcNow;

            _db.EntitySet(Key, doc2, EndDate_Modified);

            var doc3 = new Document();

            Assert.That(_db.EntityLoad(Key, doc3, EndDate_Modified), Is.True);
            Assert.That(doc2, Is.Not.EqualTo(doc3));
#if NET6_0_OR_GREATER
            Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
#endif
            Assert.That(doc2.IsDeleted, Is.EqualTo(doc3.IsDeleted));

            Assert.That(_db.EntityGet(Key, EndDate_Modified), Is.EqualTo(doc3));
            //Assert.That(_db.EntityGet<Document, IDocumentView>(Doc.Key1), Is.EqualTo(doc3));

            Assert.That(_db.EntityGet(Key, re), Is.EqualTo(doc2));
            Assert.That(_db.EntityLoad(Key, doc3, re), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            doc2.IsDeleted = true;

            Assert.That(doc2, Is.Not.EqualTo(doc3));

            Assert.That(_db.EntitySet(Key, doc2, Field_IsDeleted), Is.False);

            Assert.That(_db.EntityLoad(Key, doc3, Field_IsDeleted), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            Assert.That(_db.EntityGet(Key, Field_IsDeleted), Is.EqualTo(Document.Deleted));

        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void EntitySetGet_SingleField()
    {
        var re = _re;
        var fields = re.Fields;

        var fieldPrice = fields[nameof(Document.Price)];
        var price = 0;

        Assert.That(_db.EntitySetField<Document, long>(in Key, fieldPrice, price), Is.True);

        try
        {
            long price2 = default;
            Assert.That(_db.EntityLoadField(in NFKey, ref price2, fieldPrice), Is.False);
            Assert.That(price2, Is.Default);

            Assert.That(_db.EntityLoadField(in Key, ref price2, fieldPrice), Is.True);
            Assert.That(price2, Is.EqualTo(price));

            //var existsValue = _db.EntityGetField<Document, long>(in NFKey, fieldPrice);
            //Assert.That(existsValue.Exists, Is.False);
            //Assert.That(existsValue.Value, Is.Default);

            //existsValue = _db.EntityGetField<Document, long>(in Key, fieldPrice);
            //Assert.That(existsValue.Exists, Is.True);
            //Assert.That(existsValue.Value, Is.EqualTo(price));
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }
    
    [Test]
    public void HSET_Multi() 
    {
        var re = _re;
        var fields = re.Fields;
        var doc = new Document();
        var keys = new RedisKey[100];
        var values = fields.GetEvenFields();
        try
        {
            for (int i = 0; i < keys.Length; i++)
            {
                var key = KeyPrefix.Append(i.ToString());

                Document.New(doc, i);

                fields.ReadOddValues(values, doc);

                HSET(key, values);

                Assert.That(_db.EntityGet(key, re), Is.EqualTo(doc));

                keys[i] = key;
            }
        }
        finally
        {
            _db.KeyDelete(keys);
        }
    }

    private int HSET(RedisKey key, RedisValue[] values)
    {
        return (int)_db.ScriptEvaluate("redis.call('hset', KEYS[1], unpack(ARGV))", 
            [key], values);
    }
}