using DocLib;
using DocLib.RedisEntity;
using StackExchange.Redis.Entity.Factories;
using StackExchange.Redis.Entity.Formatters;
using System.Collections.Immutable;

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
        
        //RedisValueFormatterRegistry.RegisterEnumerableFactory(LinkedListFactory.Default, typeof(IReadOnlyCollection<>));
        RedisValueFormatterRegistry.RegisterEnumerableFactory(StackFactory.Default, typeof(IEnumerable<>), typeof(IReadOnlyCollection<>));
        RedisValueFormatterRegistry.RegisterEnumerableFactory(EquatableListFactory.Default, typeof(List<>));
        RedisValueFormatterRegistry.Register(new UnmanagedEnumerableFormatter<DocumentVersionInfos, DocumentVersionInfo>(x => new DocumentVersionInfos(x)));
        RedisValueFormatterRegistry.Register(new UnmanagedDictionaryFormatter<DocumentVersionInfoDictionary, Guid, DocumentVersionInfo>(
            x => new DocumentVersionInfoDictionary(x)));
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
            //Strings = new string?[] { "" },
            Strings = new string?[] { null, "", "test", "mystr", " ", "ascii" },
            KeyValuePairs = new Dictionary<int, int>() { { 0, 1 }, { 1, 2 } },
            Dictionary = new Dictionary<int, int>() { { 0, 1 }, { 1, 2 } },
            StringCollection = new string?[] { null, "", "test", "mystr", " ", "ascii" },
            Versions = new DocumentVersionInfoDictionary(3) { { Guid.NewGuid(), new DocumentVersionInfo(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 34) } },
#if NETCOREAPP3_1_OR_GREATER
            ImmutableList = ImmutableStack.Create(new int?[] { 0, null })
#endif
        };
        try
        {
            _db.EntitySet(Key, entity);

            var entity2 = new SimpleRecord();
            entity2.StringCollection = new Stack<string?>();

            _db.EntityLoad(entity2, Key);

#if NETCOREAPP3_1_OR_GREATER
            Assert.That(entity.ImmutableList.Cast<int?>().SequenceEqual(entity2.ImmutableList), Is.True);
            entity2.ImmutableList = entity.ImmutableList = default;
#endif
            Assert.That(entity.KeyValuePairs.SequenceEqual(entity2.KeyValuePairs), Is.True);
            Assert.That(entity.Dictionary.SequenceEqual(entity2.Dictionary), Is.True);
            Assert.That(entity.StringCollection.SequenceEqual(entity2.StringCollection), Is.True);
            Assert.That(entity.Strings.SequenceEqual(entity2.Strings), Is.True);
            Assert.That(entity.Versions.SequenceEqual(entity2.Versions), Is.True);

            entity2.Versions = entity.Versions = null;
            entity2.Dictionary = entity.Dictionary = null;
            entity2.KeyValuePairs = entity.KeyValuePairs = null;
            entity2.StringCollection = entity.StringCollection = null;
            entity2.Strings = entity.Strings = null;

            Assert.That(entity, Is.EqualTo(entity2));
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }
}