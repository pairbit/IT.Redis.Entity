using DocLib;
using DocLib.RedisEntity;
using StackExchange.Redis.Entity.Factories;
using StackExchange.Redis.Entity.Formatters;

namespace StackExchange.Redis.Entity.Tests;

public class DocumentTest
{
    private readonly IDatabase _db;

    private static readonly RedisKey KeyPrefix = "doc:";
    private static readonly RedisKey Key = KeyPrefix.Append("1");

    public DocumentTest()
    {
        var connection = ConnectionMultiplexer.Connect("localhost:6381,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
        _db = connection.GetDatabase()!;

        RedisValueFormatterRegistry.RegisterEnumerableFactory(StackFactory.Default, typeof(IEnumerable<>));
        RedisValueFormatterRegistry.RegisterEnumerableFactory(EquatableListFactory.Default, typeof(List<>));
        RedisValueFormatterRegistry.Register(new UnmanagedEnumerableFormatter<DocumentVersionInfos, DocumentVersionInfo>(x => new DocumentVersionInfos(x)));
    }

    [Test]
    public void IReadOnlyDocument_SetTest()
    {
        var doc = DocumentGenerator.New<DocumentPOCO>();
        try
        {
            _db.EntitySet<IReadOnlyDocument>(Key, doc);

            var doc2 = _db.EntityGet<DocumentPOCO, IDocument>(Key);

            Assert.That(ReferenceEquals(doc, doc2), Is.False);
            Assert.That(DocumentEqualityComparer.Default.Equals(doc, doc2), Is.True);
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void ReadOnlyDocument_SetTest()
    {
        var doc = DocumentGenerator.New<DocumentPOCO>();
        try
        {
            _db.EntitySet<IReadOnlyDocument>(Key, new ReadOnlyDocument(doc));

            var doc2 = _db.EntityGet<DocumentPOCO, IDocument>(Key);

            Assert.That(ReferenceEquals(doc, doc2), Is.False);
            Assert.That(doc2!.TagIds!.Equals(doc!.TagIds), Is.True);
            Assert.That(DocumentEqualityComparer.Default.Equals(doc, doc2), Is.True);
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void ReadOnlyDocument_LoadTest()
    {
        var doc = DocumentGenerator.New<DocumentPOCO>();
        try
        {
            _db.EntitySet<IReadOnlyDocument>(Key, new ReadOnlyDocument(doc));

            var doc2 = new DocumentPOCO();
            doc2.IntArray = new int[4];
            doc2.IntArrayN = new int?[24];
            doc2.Content = new byte[3];
            doc2.VersionInfos = new DocumentVersionInfos(100) { new DocumentVersionInfo() };
            doc2.TagIds = new EquatableList<Guid?>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            //doc2.Decimals = new ReadOnlyCollection<decimal?>(new decimal?[3]);
            //doc2.Decimals = new decimal?[4];
            //doc2.Decimals = new LinkedList<decimal?>();
            //doc2.Decimals = new Queue<decimal?>(new decimal?[] { 4534 });
            doc2.Decimals = new Stack<decimal?>(new decimal?[] { 4534 });
            doc2.Chars = new Stack<char>(new char[] { '0' });
            _db.EntityLoad<IDocument>(doc2, Key);

            Assert.That(ReferenceEquals(doc, doc2), Is.False);
            Assert.That(doc2!.TagIds!.Equals(doc!.TagIds), Is.True);
            Assert.That(DocumentEqualityComparer.Default.Equals(doc, doc2), Is.True);
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    [Test]
    public void SimpleTest()
    {
        var entity = new SimpleRecord()
        {
            Decimal = 345345345,
            DateTimeKind = DateTimeKind.Local,
            Size = DocumentSize.Medium,
            SizeLong = DocumentSizeLong.Medium,
            Strings = new string?[] { null, "", "test", "mystr", " ", "ascii" },
            StringList = new string?[] { null, "", "test", "mystr", " ", "ascii" },
        };
        try
        {
            _db.EntitySet(Key, entity);

            var entity2 = _db.EntityGet<SimpleRecord>(Key);
          
            Assert.That(entity.StringList.SequenceEqual(entity2.StringList), Is.True);
            Assert.That(entity.Strings.SequenceEqual(entity2.Strings), Is.True);

            entity2.StringList = entity.StringList = null;
            entity2.Strings = entity.Strings = null;

            Assert.That(entity, Is.EqualTo(entity2));
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }
}