﻿using DocLib.RedisEntity;
using IT.Redis.Entity.Configurations;
using System.Text;

namespace IT.Redis.Entity.Tests;

public class DocumentDependTest
{
    private readonly IDatabase _db;

    private static readonly RedisKey KeyPrefix = "doc:";
    private static readonly RedisKey Key = KeyPrefix.Append("1");

    static DocumentDependTest()
    {
        RedisEntity.Config = new AnnotationConfiguration(RedisValueFormatterRegistry.Default);
    }

    public DocumentDependTest()
    {
        var connection = ConnectionMultiplexer.Connect(Const.Connection);
        _db = connection.GetDatabase()!;
    }

    [Test]
    public void Nullable_NotNullable()
    {
        try
        {
            _db.EntitySet(Key, new DocumentNullable());

            var doc2 = _db.EntityGet<DocumentNotNullable>(Key)!;

            _db.EntitySet(Key, doc2);

            var doc = _db.EntityGet<DocumentNullable>(Key)!;
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }
    
    //[Test]
    //public void RedisValue_Zero()
    //{
    //    RedisValue zero = RedisValue.EmptyString;

    //    byte[] zeroBytes = zero;
    //    byte[]? zeroBytesNull = zero;

    //    string zeroString = zero;
    //    string? zeroStringNull = zero;

    //    ReadOnlyMemory<byte> memBytes = zero;
    //    ReadOnlyMemory<byte>? memBytesNull = zero;
    //}
    
    [Test]
    public void DependData()
    {
        try
        {
            var doc = new DocumentDepend();
            DocumentDepend.New(doc, 1);

            _db.EntitySet(Key, doc);

            var doc2 = new DocumentDepend();
            doc2.NumCollection = new List<int>() { 4};
            
            _db.EntityLoad(Key, doc2);

            if (doc.Content != null && doc2.Content != null)
            {
                Assert.That(doc.Content!.SequenceEqual(doc2.Content!), Is.True);
                doc2.Content = doc.Content;
            }

            if (doc.MemoryBytesNull != null && doc2.MemoryBytesNull != null)
            {
                Assert.That(doc.MemoryBytesNull.Value.Span.SequenceEqual(doc2.MemoryBytesNull.Value.Span), Is.True);
                doc2.MemoryBytesNull = doc.MemoryBytesNull;
            }

            Assert.That(doc.MemoryBytes.Span.SequenceEqual(doc2.MemoryBytes.Span), Is.True);

            doc2.MemoryBytes = doc.MemoryBytes;

            Assert.That(doc.NumCollection!.SequenceEqual(doc2.NumCollection!), Is.True);
            doc.NumCollection = doc2.NumCollection;

            Assert.That(doc.Nums!.SequenceEqual(doc2.Nums!), Is.True);
            doc.Nums = doc2.Nums;

            Assert.That(doc.Bits!.Cast<bool>().SequenceEqual(doc2.Bits!.Cast<bool>()), Is.True);
            doc.Bits = doc2.Bits;

            Assert.That(doc, Is.EqualTo(doc2));
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void ReadKeyTest()
    {
        var id = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        string name = "3";

        var redisKey = $"doc:{id:N}:{clientId:N}:{name}:4";
        var length = redisKey.Length;

        var doc = new DocumentWithReadOnlyKeys(id, clientId, name, "4")
        {
            Key5 = "5",
            Data1 = "data-test1"
        };

        var re = RedisEntity<DocumentWithReadOnlyKeys>.Default;

        Assert.That(doc.RedisKey, Is.Null);

        try
        {
            _db.EntitySet(doc, re.Fields[nameof(DocumentWithReadOnlyKeys.Data1)]);

            Assert.That(doc.RedisKey, Is.Not.Null);
            Assert.That(doc.RedisKey.Length, Is.EqualTo(length));
            Assert.That(doc.RedisKey.SequenceEqual(Encoding.UTF8.GetBytes(redisKey)), Is.True);

            doc.Data2 = "test-data2";

            _db.EntitySet(doc, re.Fields[nameof(DocumentWithReadOnlyKeys.Data2)]);
        }
        finally
        {
            _db.KeyDelete(doc.RedisKey);
        }
    }
}