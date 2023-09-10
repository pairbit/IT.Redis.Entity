using DocLib.RedisEntity;
using System;

namespace StackExchange.Redis.Entity.Tests;

public class DocumentDependTest
{
    private readonly IDatabase _db;

    private static readonly RedisKey KeyPrefix = "doc:";
    private static readonly RedisKey Key = KeyPrefix.Append("1");

    public DocumentDependTest()
    {
        var connection = ConnectionMultiplexer.Connect("localhost:6381,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
        _db = connection.GetDatabase()!;
    }

    [Test]
    public void DependData()
    {
        try
        {
            var doc = new DocumentDepend();
            DocumentDepend.New(doc, 1);

            _db.EntitySet(Key, doc);

            var doc2 = _db.EntityGet<DocumentDepend>(Key)!;

            if (doc.Content != null && doc2.Content != null)
            {
                Assert.That(doc.Content!.SequenceEqual(doc2.Content!), Is.True);
                doc2.Content = doc.Content;
            }

            Assert.That(doc.MemoryBytes.Span.SequenceEqual(doc2.MemoryBytes.Span), Is.True);

            doc2.MemoryBytes = doc.MemoryBytes;
            Assert.That(doc, Is.EqualTo(doc2));
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }
}