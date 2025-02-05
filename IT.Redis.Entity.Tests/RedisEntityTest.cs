using DocLib;
using IT.Redis.Entity.Configurations;

namespace IT.Redis.Entity.Tests;

public class RedisEntityTest
{
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
        _re = re;
    }

    [Test]
    public void HashSet_Multi()
    {
        var re = _re;
        var fields = re.Fields.Array;
        var redisFields = re.Fields.RedisValues;
        var entries = new HashEntry[fields.Length];
        var document = new Document();
        var keys = new RedisKey[100];
        var db = Shared.Db;
        try
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Document.New(document, i);

                fields.ReadEntries(entries, document);

                var key = KeyPrefix.Append(i.ToString());

                db.HashSet(key, entries);

                Assert.That(fields.GetEntity(db.HashGet(key, redisFields)), Is.EqualTo(document));

                keys[i] = key;
            }
        }
        finally
        {
            db.KeyDelete(keys);
        }
    }

    [Test]
    public void HashGet_Multi()
    {
        var db = Shared.Db;
        var re = _re;
        var fields = re.Fields.Array;
        var redisFields = re.Fields.RedisValues;
        try
        {
            db.EntitySet(Key, Document.Data, re);

            var documents = new Document?[10];

            var values = db.HashGet(Key, redisFields);

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
            db.KeyDelete(Key);
        }
    }

    [Test]
    public void HashSetGet()
    {
        var db = Shared.Db;
        var re = _re;
        var fields = re.Fields;
        var EndDate_Modified = fields.Sub(
#if NET6_0_OR_GREATER
            nameof(Document.EndDate),
#endif
            nameof(Document.Modified));

        var Field_IsDeleted = fields[nameof(Document.IsDeleted)];

        Assert.That(fields.Array.GetEntity(db.HashGet(Key, fields.RedisValues)), Is.Null);
        Assert.That(EndDate_Modified.Array.GetEntity(db.HashGet(Key, EndDate_Modified.RedisValues)), Is.Null);

        var doc2 = new Document();

        Assert.That(EndDate_Modified.Array.Write(doc2, db.HashGet(Key, EndDate_Modified.RedisValues)), Is.False);
        Assert.That(Field_IsDeleted.Write(doc2, db.HashGet(Key, Field_IsDeleted.RedisValue)), Is.False);

        Assert.That(doc2, Is.EqualTo(Document.Empty));

        try
        {
            db.HashSet(Key, fields.Array.GetEntries(Document.Data));

            Assert.That(fields.Array.Write(doc2, db.HashGet(Key, fields.RedisValues)), Is.True);

            Assert.That(doc2, Is.EqualTo(Document.Data));
            Assert.That(fields.Array.GetEntity(db.HashGet(Key, fields.RedisValues)), Is.EqualTo(Document.Data));
#if NET6_0_OR_GREATER
            doc2.EndDate = new DateOnly(2022, 03, 20);
#endif
            doc2.Modified = DateTime.UtcNow;

            db.HashSet(Key, EndDate_Modified.Array.GetEntries(doc2));

            var doc3 = new Document();

            Assert.That(EndDate_Modified.Array.Write(doc3, db.HashGet(Key, EndDate_Modified.RedisValues)), Is.True);

            Assert.That(doc2, Is.Not.EqualTo(doc3));
#if NET6_0_OR_GREATER
            Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
#endif
            Assert.That(doc2.Modified, Is.EqualTo(doc3.Modified));

            Assert.That(fields.Array.Write(doc3, db.HashGet(Key, fields.RedisValues)), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            doc2.IsDeleted = true;

            Assert.That(doc2, Is.Not.EqualTo(doc3));

            Assert.That(db.HashSet(Key, Field_IsDeleted.RedisValue, Field_IsDeleted.Read(doc2)), Is.False);

            Assert.That(Field_IsDeleted.Write(doc3, db.HashGet(Key, Field_IsDeleted.RedisValue)), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            Assert.That(Field_IsDeleted.GetEntity(db.HashGet(Key, Field_IsDeleted.RedisValue)), Is.EqualTo(Document.Deleted));

        }
        finally
        {
            db.KeyDelete(Key);
        }
    }

    [Test]
    public void EntitySetGet()
    {
        var db = Shared.Db;
        var re = _re;
        var EndDate_Modified = re.Sub(
#if NET6_0_OR_GREATER
            nameof(Document.EndDate),
#endif
            nameof(Document.Modified));

        var Field_IsDeleted = re.Fields[nameof(Document.IsDeleted)];

        Assert.That(db.EntityGet<Document>(Key, Field_IsDeleted), Is.Null);
        Assert.That(db.EntityGet<Document>(Key, EndDate_Modified), Is.Null);
        Assert.That(db.EntityGet<Document>(Key), Is.Null);

        var doc2 = new Document();

        Assert.That(db.EntityLoad(Key, doc2, Field_IsDeleted), Is.False);
        Assert.That(db.EntityLoad(Key, doc2, EndDate_Modified), Is.False);
        Assert.That(db.EntityLoad(Key, doc2), Is.False);

        Assert.That(doc2, Is.EqualTo(Document.Empty));

        try
        {
            db.EntitySet(Key, Document.Data, re);

            Assert.That(db.EntityLoad(Key, doc2, re), Is.True);

            Assert.That(doc2, Is.EqualTo(Document.Data));
            Assert.That(db.EntityGet(Key, re), Is.EqualTo(Document.Data));
#if NET6_0_OR_GREATER
            doc2.EndDate = new DateOnly(2022, 03, 20);
#endif
            doc2.Modified = DateTime.UtcNow;

            db.EntitySet(Key, doc2, EndDate_Modified);

            var doc3 = new Document();

            Assert.That(db.EntityLoad(Key, doc3, EndDate_Modified), Is.True);
            Assert.That(doc2, Is.Not.EqualTo(doc3));
#if NET6_0_OR_GREATER
            Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
#endif
            Assert.That(doc2.IsDeleted, Is.EqualTo(doc3.IsDeleted));

            Assert.That(db.EntityGet(Key, EndDate_Modified), Is.EqualTo(doc3));
            //Assert.That(_db.EntityGet<Document, IDocumentView>(Doc.Key1), Is.EqualTo(doc3));

            Assert.That(db.EntityGet(Key, re), Is.EqualTo(doc2));
            Assert.That(db.EntityLoad(Key, doc3, re), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            doc2.IsDeleted = true;

            Assert.That(doc2, Is.Not.EqualTo(doc3));

            Assert.That(db.EntitySet(Key, doc2, Field_IsDeleted), Is.False);

            Assert.That(db.EntityLoad(Key, doc3, Field_IsDeleted), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            Assert.That(db.EntityGet(Key, Field_IsDeleted), Is.EqualTo(Document.Deleted));

        }
        finally
        {
            db.KeyDelete(Key);
        }
    }

    [Test]
    public void EntitySetGet_SingleField()
    {
        var db = Shared.Db;
        var re = _re;
        var fields = re.Fields;

        var fieldPrice = fields[nameof(Document.Price)];
        var price = 0;

        Assert.That(db.EntitySetField<Document, long>(in Key, fieldPrice, price), Is.True);

        try
        {
            long price2 = default;
            Assert.That(db.EntityLoadField(in NFKey, ref price2, fieldPrice), Is.False);
            Assert.That(price2, Is.Default);

            Assert.That(db.EntityLoadField(in Key, ref price2, fieldPrice), Is.True);
            Assert.That(price2, Is.EqualTo(price));

            Assert.That(db.EntityGetField<Document, long>(in NFKey, fieldPrice, defaultValue: -1), Is.EqualTo(-1));
            Assert.That(db.EntityGetField<Document, long>(in Key, fieldPrice), Is.EqualTo(price));
        }
        finally
        {
            db.KeyDelete(Key);
        }
    }
    
    [Test]
    public void HSET_Multi() 
    {
        var db = Shared.Db;
        var re = _re;
        var fields = re.Fields.Array;
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

                Assert.That(db.EntityGet(key, re), Is.EqualTo(doc));

                keys[i] = key;
            }
        }
        finally
        {
            db.KeyDelete(keys);
        }
    }

    private int HSET(RedisKey key, RedisValue[] values)
    {
        var db = Shared.Db;
        return (int)db.ScriptEvaluate("redis.call('hset', KEYS[1], unpack(ARGV))", 
            [key], values);
    }
}