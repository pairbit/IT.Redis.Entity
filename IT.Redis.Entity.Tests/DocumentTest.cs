using DocLib;
using DocLib.RedisEntity;
using IT.Collections.Factory;
using IT.Collections.Factory.Factories;
using IT.Redis.Entity.Formatters;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace IT.Redis.Entity.Tests;

public class DocumentTest
{
    private readonly IDatabase _db;

    private static readonly RedisKey KeyPrefix = "doc:";
    private static readonly RedisKey Key = KeyPrefix.Append("1");

    static DocumentTest()
    {
        RedisEntity.Factory = new RedisEntityFactory(new RedisEntityConfiguration(RedisValueFormatterRegistry.Default));
    }

    public DocumentTest()
    {
        var connection = ConnectionMultiplexer.Connect("localhost:6381,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
        _db = connection.GetDatabase()!;

        //RedisValueFormatterRegistry.RegisterEnumerableFactory(LinkedListFactory.Default, typeof(IReadOnlyCollection<>));
        //RedisValueFormatterRegistry.RegisterEnumerableFactory(StackFactory.Default, typeof(IEnumerable<>), typeof(IReadOnlyCollection<>));
        RedisValueFormatterRegistry.EnumerableFactoryRegistry.TryRegisterFactory<ListFactory>(EquatableListFactory.Default, RegistrationBehavior.OverwriteExisting);

        RedisValueFormatterRegistry.Register(new UnmanagedEnumerableFormatter<DocumentVersionInfos, DocumentVersionInfo>(x => new DocumentVersionInfos(x), (x, item) => x.Add(item)));
        RedisValueFormatterRegistry.Register(new UnmanagedDictionaryFormatter<DocumentVersionInfoDictionary, Guid, DocumentVersionInfo>(
            x => new DocumentVersionInfoDictionary(x), (x, item) => x.Add(item.Key, item.Value)));
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
            ProducerConsumerCollection = new ConcurrentBag<string?>() { "0", "1" },
            ConcurrentBag = new ConcurrentBag<string?>() { "2", "3" },
            ConcurrentQueue = new ConcurrentQueue<string?>(new string[] { "4", "5" }),
            ConcurrentStack = new ConcurrentStack<string?>(new string[] { "6", "7" }),
            BlockingCollection = new BlockingCollection<string?>(new ConcurrentQueue<string?>(new string?[] { "8", "9" })),
            StringPairs = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("a1", "b"), new KeyValuePair<string, string>("c2", "d") },
            StringDictionary = new Dictionary<string, string>() { { "g", "h" }, { "j", "k" } },
            StringArray = new [] { new KeyValuePair<string, string>("a12", "b"), new KeyValuePair<string, string>("c25", "d") },
#if NETCOREAPP3_1_OR_GREATER
            ImmutableList = ImmutableStack.Create(new int?[] { 0, null }),
            Tuple = (0, -1),
            NullableTuple = (null, -1),
#endif
        };
        try
        {
            _db.EntitySet(Key, entity);

            var entity2 = new SimpleRecord();
            entity2.StringCollection = new Stack<string?>();
            entity2.ProducerConsumerCollection = new ConcurrentBag<string?>();
            entity2.ConcurrentBag = new ConcurrentBag<string?>();
            entity2.ConcurrentQueue = new ConcurrentQueue<string?>();
            entity2.ConcurrentStack = new ConcurrentStack<string?>(new string?[] { "3" });
            entity2.BlockingCollection = new BlockingCollection<string?>();
            entity2.StringPairs = new List<KeyValuePair<string, string>>();
            entity2.StringDictionary = new Dictionary<string, string>() { { "4", "5" } };
            _db.EntityLoad(entity2, Key);

#if NETCOREAPP3_1_OR_GREATER
            Assert.That(entity.ImmutableList!.Cast<int?>().SequenceEqual(entity2.ImmutableList!), Is.True);
            Assert.That(entity.Tuple, Is.EqualTo(entity2.Tuple));
            Assert.That(entity.NullableTuple, Is.EqualTo(entity2.NullableTuple));
#endif
            Assert.That(entity.KeyValuePairs.SequenceEqual(entity2.KeyValuePairs!), Is.True);
            Assert.That(entity.Dictionary.SequenceEqual(entity2.Dictionary!), Is.True);
            Assert.That(entity.ReadOnlyDictionary.SequenceEqual(entity2.ReadOnlyDictionary!), Is.True);
            Assert.That(entity.SortedDictionary.SequenceEqual(entity2.SortedDictionary!), Is.True);
            Assert.That(entity.SortedList.SequenceEqual(entity2.SortedList!), Is.True);
            Assert.That(entity.StringCollection.SequenceEqual(entity2.StringCollection!), Is.True);
            Assert.That(entity.Strings.SequenceEqual(entity2.Strings!), Is.True);
            Assert.That(entity.Versions.SequenceEqual(entity2.Versions!), Is.True);
            Assert.That(entity.ConcurrentDictionary.SequenceEqual(entity2.ConcurrentDictionary!), Is.True);
            Assert.That(entity.ConcurrentQueue.SequenceEqual(entity2.ConcurrentQueue), Is.True);
            Assert.That(entity.BlockingCollection.SequenceEqual(entity2.BlockingCollection), Is.True);

            Assert.That(entity.ProducerConsumerCollection.SequenceEqual(entity2.ProducerConsumerCollection), Is.True);
            Assert.That(entity.ConcurrentBag.SequenceEqual(entity2.ConcurrentBag), Is.True);
            Assert.That(entity.ConcurrentStack.SequenceEqual(entity2.ConcurrentStack), Is.True);

            Assert.That(entity.StringPairs.SequenceEqual(entity2.StringPairs), Is.True);
            Assert.That(entity.StringDictionary.SequenceEqual(entity2.StringDictionary), Is.True);
            Assert.That(entity.StringArray.SequenceEqual(entity2.StringArray!), Is.True);

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

    public record MyRecInt
    {
        public int MyInt { get; set; }
    }

    [Test]
    public void Int32Test()
    {
        var doc = new MyRecInt { MyInt = int.MinValue };

        try
        {
            _db.EntitySet(Key, doc);

            var doc2 = _db.EntityGet<MyRecInt>(Key);
        }
        finally
        {
            _db.KeyDelete(Key);
        }
    }

    //[Test]
    public void ReadKeyTest()
    {
        var doc = new DocumentAnnotation
        {
            Id = Guid.NewGuid(),
            Name = "My Doc 1",
            AttachmentIds = new EquatableList<int> { 0, 1, 3, 5 }
        };

        _db.EntitySet(doc);
    }
}