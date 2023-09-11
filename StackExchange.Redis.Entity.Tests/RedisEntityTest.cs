using DocLib;

namespace StackExchange.Redis.Entity.Tests;

public abstract class RedisEntityTest
{
    private readonly IDatabase _db;
    private readonly IRedisEntityWriter<Document> _writer;
    private readonly IRedisEntityReader<Document> _reader;

    private static readonly RedisKey KeyPrefix = "doc:";
    private static readonly RedisKey Key = KeyPrefix.Append("1");

    public RedisEntityTest(IRedisEntityWriter<Document> writer, IRedisEntityReader<Document> reader)
    {
        var connection = ConnectionMultiplexer.Connect("localhost:6381,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
        _db = connection.GetDatabase()!;
        _writer = writer;
        _reader = reader;
    }

    [Test]
    public void HashSet_Multi()
    {
        var reader = _reader;
        var writer = _writer;
        var entries = new HashEntry[reader.Fields.All.Length];
        var document = new Document();
        var keys = new RedisKey[100];

        try
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Document.New(document, i);

                reader.Read(entries, document);

                var key = KeyPrefix.Append(i.ToString());

                _db.HashSet(key, entries);

                Assert.That(writer.GetEntity(_db.HashGetAll(key)), Is.EqualTo(document));

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
        var reader = _reader;
        var writer = _writer;

        try
        {
            _db.EntitySet(Key, Document.Data, reader);

            var documents = new Document?[10];

            var entries = _db.HashGetAll(Key);

            for (int i = 0; i < documents.Length; i++)
            {
                documents[i] = writer.GetEntity(entries);
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
        var reader = _reader;
        var writer = _writer;

        var EndDate_Modified = new RedisValue[] { reader.Fields[nameof(Document.EndDate)], reader.Fields[nameof(Document.Modified)] };
        var Field_IsDeleted = reader.Fields[nameof(Document.IsDeleted)];

        Assert.IsNull(writer.GetEntity(_db.HashGetAll(Key)));
        Assert.IsNull(writer.GetEntity(EndDate_Modified, _db.HashGet(Key, EndDate_Modified)));
        Assert.IsNull(writer.GetEntity(writer.Fields.All, _db.HashGet(Key, writer.Fields.All)));

        var doc2 = new Document();

        Assert.That(writer.Write(doc2, _db.HashGetAll(Key)), Is.False);
        Assert.That(writer.Write(doc2, EndDate_Modified, _db.HashGet(Key, EndDate_Modified)), Is.False);
        Assert.That(writer.Write(doc2, Field_IsDeleted, _db.HashGet(Key, Field_IsDeleted)), Is.False);

        Assert.That(doc2, Is.EqualTo(Document.Empty));

        try
        {
            _db.HashSet(Key, reader.GetEntries(Document.Data));

            Assert.That(writer.Write(doc2, _db.HashGetAll(Key)), Is.True);

            Assert.That(doc2, Is.EqualTo(Document.Data));
            Assert.That(writer.GetEntity(_db.HashGetAll(Key)), Is.EqualTo(Document.Data));

            doc2.EndDate = new DateOnly(2022, 03, 20);
            doc2.Modified = DateTime.UtcNow;

            _db.HashSet(Key, reader.GetEntries(doc2, EndDate_Modified));

            var doc3 = new Document();

            Assert.That(writer.Write(doc3, EndDate_Modified, _db.HashGet(Key, EndDate_Modified)), Is.True);

            Assert.That(doc2, Is.Not.EqualTo(doc3));
            Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
            Assert.That(doc2.Modified, Is.EqualTo(doc3.Modified));

            Assert.That(writer.Write(doc3, writer.Fields.All, _db.HashGet(Key, writer.Fields.All)), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            doc2.IsDeleted = true;

            Assert.That(doc2, Is.Not.EqualTo(doc3));

            Assert.That(_db.HashSet(Key, Field_IsDeleted, reader.Read(doc2, Field_IsDeleted)), Is.False);

            Assert.That(writer.Write(doc3, Field_IsDeleted, _db.HashGet(Key, Field_IsDeleted)), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            Assert.That(writer.GetEntity(Field_IsDeleted, _db.HashGet(Key, Field_IsDeleted)), Is.EqualTo(Document.Deleted));

        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void EntitySetGet()
    {
        var reader = _reader;
        var writer = _writer;

        var EndDate_Modified = new RedisValue[] { reader.Fields[nameof(Document.EndDate)], reader.Fields[nameof(Document.Modified)] };
        var Field_IsDeleted = reader.Fields[nameof(Document.IsDeleted)];

        Assert.IsNull(_db.EntityGetAll<Document>(Key));
        Assert.IsNull(_db.EntityGet<Document>(Key, Field_IsDeleted));
        Assert.IsNull(_db.EntityGet<Document>(Key, EndDate_Modified));
        Assert.IsNull(_db.EntityGet<Document>(Key, writer));

        var doc2 = new Document();

        Assert.That(_db.EntityLoadAll(doc2, Key), Is.False);
        Assert.That(_db.EntityLoad(doc2, Key, Field_IsDeleted, writer), Is.False);
        Assert.That(_db.EntityLoad(doc2, Key, EndDate_Modified), Is.False);
        Assert.That(_db.EntityLoad(doc2, Key, writer), Is.False);

        Assert.That(doc2, Is.EqualTo(Document.Empty));

        try
        {
            _db.EntitySet(Key, Document.Data, reader);

            Assert.That(_db.EntityLoadAll(doc2, Key, writer), Is.True);

            Assert.That(doc2, Is.EqualTo(Document.Data));
            Assert.That(_db.EntityGetAll(Key, writer), Is.EqualTo(Document.Data));

            doc2.EndDate = new DateOnly(2022, 03, 20);
            doc2.Modified = DateTime.UtcNow;

            _db.EntitySet(Key, doc2, EndDate_Modified, reader);

            var doc3 = new Document();

            Assert.That(_db.EntityLoad(doc3, Key, EndDate_Modified, writer), Is.True);
            Assert.That(doc2, Is.Not.EqualTo(doc3));
            Assert.That(doc2.EndDate, Is.EqualTo(doc3.EndDate));
            Assert.That(doc2.IsDeleted, Is.EqualTo(doc3.IsDeleted));

            Assert.That(_db.EntityGet(Key, EndDate_Modified, writer), Is.EqualTo(doc3));
            //Assert.That(_db.EntityGet<Document, IDocumentView>(Doc.Key1), Is.EqualTo(doc3));

            Assert.That(_db.EntityGet(Key, writer), Is.EqualTo(doc2));
            Assert.That(_db.EntityLoad(doc3, Key, writer), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            doc2.IsDeleted = true;

            Assert.That(doc2, Is.Not.EqualTo(doc3));

            Assert.That(_db.EntitySet(Key, doc2, Field_IsDeleted, reader), Is.False);

            Assert.That(_db.EntityLoad(doc3, Key, Field_IsDeleted, writer), Is.True);

            Assert.That(doc2, Is.EqualTo(doc3));

            Assert.That(_db.EntityGet(Key, Field_IsDeleted, writer), Is.EqualTo(Document.Deleted));

        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }
}