using DocLib;
using DocLib.RedisEntity;
using StackExchange.Redis.Entity.Factories;
using StackExchange.Redis.Entity.Formatters;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

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
            KeyValuePairs = new Dictionary<int, int?>() { { 0, 1 }, { 1, null } },
            Dictionary = new Dictionary<int, int>() { { 0, 1 }, { 1, 2 } },
            ReadOnlyDictionary = new ReadOnlyDictionary<int, int?>(new Dictionary<int, int?>() { { 0, null }, { 1, 2 } }),
            SortedDictionary = new SortedDictionary<int, int>() { { 4, 1 }, { 1, 2 } },
            SortedList = new SortedList<int, int?>() { { 9, null }, { 0, 8 } },
            StringCollection = new string?[] { null, "", "test", "mystr", " ", "ascii" },
            Versions = new DocumentVersionInfoDictionary(3) { { Guid.NewGuid(), new DocumentVersionInfo(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 34) } },
            ConcurrentDictionary = new ConcurrentDictionary<int, int>(new Dictionary<int, int>() { { 0, 1 }, { 1, 2 } }),
            ProducerConsumerCollection = new ConcurrentBag<int>() { 6, 9, 1 },
            ConcurrentBag = new ConcurrentBag<int>() { 5, 7, 2 },
            ConcurrentQueue = new ConcurrentQueue<int>(new int[] { 1, 2, 3 }),
            ConcurrentStack = new ConcurrentStack<int>(new int[] { 4, 5, 6 }),
            BlockingCollection = new BlockingCollection<int>(new ConcurrentQueue<int>(new int[] { 7, 8, 9 })),
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
#endif
            Assert.That(entity.KeyValuePairs.SequenceEqual(entity2.KeyValuePairs), Is.True);
            Assert.That(entity.Dictionary.SequenceEqual(entity2.Dictionary), Is.True);
            Assert.That(entity.ReadOnlyDictionary.SequenceEqual(entity2.ReadOnlyDictionary), Is.True);
            Assert.That(entity.SortedDictionary.SequenceEqual(entity2.SortedDictionary), Is.True);
            Assert.That(entity.SortedList.SequenceEqual(entity2.SortedList), Is.True);
            Assert.That(entity.StringCollection.SequenceEqual(entity2.StringCollection), Is.True);
            Assert.That(entity.Strings.SequenceEqual(entity2.Strings), Is.True);
            Assert.That(entity.Versions.SequenceEqual(entity2.Versions), Is.True);
            Assert.That(entity.ConcurrentDictionary.SequenceEqual(entity2.ConcurrentDictionary), Is.True);
            Assert.That(entity.ConcurrentQueue.SequenceEqual(entity2.ConcurrentQueue), Is.True);
            Assert.That(entity.BlockingCollection.SequenceEqual(entity2.BlockingCollection), Is.True);

            Assert.That(entity.ProducerConsumerCollection.OrderBy(x => x).SequenceEqual(entity2.ProducerConsumerCollection.OrderBy(x => x)), Is.True);
            Assert.That(entity.ConcurrentBag.OrderBy(x => x).SequenceEqual(entity2.ConcurrentBag.OrderBy(x => x)), Is.True);
            Assert.That(entity.ConcurrentStack.OrderBy(x => x).SequenceEqual(entity2.ConcurrentStack.OrderBy(x => x)), Is.True);

            Assert.That(entity.Decimal, Is.EqualTo(entity2.Decimal));
            Assert.That(entity.DateTimeKind, Is.EqualTo(entity2.DateTimeKind));
            Assert.That(entity.Size, Is.EqualTo(entity2.Size));
            Assert.That(entity.SizeLong, Is.EqualTo(entity2.SizeLong));
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }
}