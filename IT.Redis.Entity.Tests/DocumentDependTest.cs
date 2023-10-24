using DocLib.RedisEntity;

namespace IT.Redis.Entity.Tests;

public class DocumentDependTest
{
    private readonly IDatabase _db;

    private static readonly RedisKey KeyPrefix = "doc:";
    private static readonly RedisKey Key = KeyPrefix.Append("1");

    static DocumentDependTest()
    {
        RedisEntity.Factory = new RedisEntityFactory(new RedisEntityConfiguration(RedisValueFormatterRegistry.Default));
    }

    public DocumentDependTest()
    {
        var connection = ConnectionMultiplexer.Connect("localhost:6381,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
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
            
            _db.EntityLoad(doc2, Key);

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
        var name = "MyDoc1";

        var redisKey = $"dep:doc:{id:N}:{clientId:N}:{name}";
        var length = redisKey.Length;

        var doc = new DocumentWithKeys
        {
            Id = id,
            ClientId = clientId,
            Name = name,
            Data1 = "data-test1"
        };

        Assert.That(doc.RedisKey, Is.Null);

        var reader = RedisEntity<DocumentWithKeys>.Reader;

        try
        {
            _db.EntitySet(doc, reader.Fields[nameof(DocumentWithKeys.Data1)]);
            
            Assert.That(doc.RedisKey, Is.Not.Null);
            Assert.That(doc.RedisKey.Length, Is.EqualTo(length));

            doc.Data2 = "test-data2";

            _db.EntitySet(doc, reader.Fields[nameof(DocumentWithKeys.Data2)]);
        }
        finally
        {
            _db.KeyDelete(doc.RedisKey);
        }
    }
}