using DocLib;
using IT.Collections.Equatable;
using IT.Collections.Equatable.Factory;
using IT.Collections.Factory;
using IT.Redis.Entity.Formatters;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

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
        RedisValueFormatterRegistry.EnumerableFactoryRegistry.RegisterFactoriesEquatable(RegistrationBehavior.OverwriteExisting);

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
            _db.EntityLoad<IDocument>(Key, doc2);

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
            _db.EntityLoad(Key, entity2);

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

    [Test]
    public void ReadKeyTest()
    {
        var doc = new DocumentAnnotation
        {
            Id = Guid.NewGuid(),
            Name = "My Doc 1",
            AttachmentIds = new EquatableList<int> { 0, 1, 3, 5 }
        };

        var reader = RedisEntity<DocumentAnnotation>.Reader;

        try
        {
            _db.EntitySet(doc, reader.Fields[nameof(DocumentAnnotation.AttachmentIds)]);

            var doc2 = new DocumentAnnotation();

            Assert.That(_db.EntityLoad(doc2), Is.False);

            doc2.Id = doc.Id;

            Assert.That(_db.EntityLoad(doc2), Is.True);
            Assert.That(doc.AttachmentIds, Is.EqualTo(doc2.AttachmentIds));

            _db.EntitySet(doc, reader.Fields[nameof(DocumentAnnotation.Name)]);
        }
        finally
        {
            _db.KeyDelete(doc.RedisKey);
        }
    }
    
    [Test]
    public void RedisKeyBuilderKey8()
    {
        var builder = RedisKeyBuilder.Default;

        var key = builder.BuildKey(null, 255, 0, 1, 2, 3, 4, 5, 6, 7, 8);

        Assert.That(key.SequenceEqual(U8("1:2:3:4:5:6:7:8")), Is.True);
    }

    [Test]
    public void RedisKeyBuilderKey()
    {
        var builder = RedisKeyBuilder.Default;

        var id = Guid.NewGuid();
        var idn = id.ToString("N");
        var idb = Encoding.UTF8.GetBytes(idn);
        var idWithPrefix = Encoding.UTF8.GetBytes("prefix:" + idn);
        var prefix = Encoding.UTF8.GetBytes("prefix:");
        var empty = new byte[idb.Length];
        var emptyWithPostfix = new byte[idb.Length + 1];

        Assert.That(builder.BuildKey(empty, 0, 0, id) == empty, Is.True);
        Assert.That(builder.BuildKey(null, 0, 0, id).SequenceEqual(empty), Is.True);
        Assert.That(builder.BuildKey(Array.Empty<byte>(), 0, 0, id).SequenceEqual(empty), Is.True);
        Assert.That(builder.BuildKey(new byte[10], 0, 0, id).SequenceEqual(empty), Is.True);
        Assert.That(builder.BuildKey(emptyWithPostfix, 0, 0, id).SequenceEqual(empty), Is.True);

        Assert.That(builder.BuildKey(null, 1, 0, id).SequenceEqual(idb), Is.True);

        var withOffset = new byte[prefix.Length + idb.Length];
        Assert.That(builder.BuildKey(null, 0, prefix.Length, id).SequenceEqual(withOffset), Is.True);

        idb.CopyTo(withOffset.AsSpan(prefix.Length));

        var key = builder.BuildKey(null, 1, prefix.Length, id);
        Assert.That(key.SequenceEqual(withOffset), Is.True);
        
        prefix.CopyTo(key.AsSpan());
        prefix.CopyTo(withOffset.AsSpan());
        Assert.That(key.SequenceEqual(withOffset), Is.True);
        Assert.That(key.SequenceEqual(idWithPrefix), Is.True);

        //change key

        Assert.That(builder.BuildKey(key, 0, prefix.Length, id) == key, Is.True);
        Assert.That(key.SequenceEqual(idWithPrefix), Is.True);

        Assert.That(builder.BuildKey(key, 1, prefix.Length, id) == key, Is.True);
        Assert.That(key.SequenceEqual(idWithPrefix), Is.True);

        id = Guid.NewGuid();
        idWithPrefix = Encoding.UTF8.GetBytes($"prefix:{id:N}");
        Assert.That(builder.BuildKey(key, 1, prefix.Length, id).SequenceEqual(idWithPrefix), Is.True);
    }

    [Test]
    public void BitsTest()
    {
        byte bits = 0;
        Assert.That(bits |= 1 << 0, Is.EqualTo(1));
        Assert.That(bits |= 1 << 1, Is.EqualTo(3));
        Assert.That(bits |= 1 << 2, Is.EqualTo(7));
        Assert.That(bits |= 1 << 3, Is.EqualTo(15));
        Assert.That(bits |= 1 << 4, Is.EqualTo(31));
        Assert.That(bits |= 1 << 5, Is.EqualTo(63));
        Assert.That(bits |= 1 << 6, Is.EqualTo(127));
        Assert.That(bits |= 1 << 7, Is.EqualTo(255));

        Assert.That(bits = 0, Is.EqualTo(0));
        Assert.That(bits |= 1, Is.EqualTo(1)); Assert.That(bits & 1, Is.EqualTo(1));
        Assert.That(bits |= 2, Is.EqualTo(3)); Assert.That(bits & 2, Is.EqualTo(2));
        Assert.That(bits |= 4, Is.EqualTo(7)); Assert.That(bits & 4, Is.EqualTo(4));
        Assert.That(bits |= 8, Is.EqualTo(15)); Assert.That(bits & 8, Is.EqualTo(8));
        Assert.That(bits |= 16, Is.EqualTo(31)); Assert.That(bits & 16, Is.EqualTo(16));
        Assert.That(bits |= 32, Is.EqualTo(63)); Assert.That(bits & 32, Is.EqualTo(32));
        Assert.That(bits |= 64, Is.EqualTo(127)); Assert.That(bits & 64, Is.EqualTo(64));
        Assert.That(bits |= 128, Is.EqualTo(255)); Assert.That(bits & 128, Is.EqualTo(128));
    }

    private byte[] U8(string str) => Encoding.UTF8.GetBytes(str);
}