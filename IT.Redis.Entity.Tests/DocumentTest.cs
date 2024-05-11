using DocLib;
using DocLib.RedisEntity;
using IT.Collections.Equatable;
using IT.Collections.Equatable.Factory;
using IT.Collections.Factory;
using IT.Redis.Entity.Configurations;
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
        RedisEntity.Factory = new RedisEntityFactory(new AnnotationConfiguration(RedisValueFormatterRegistry.Default));
    }

    public DocumentTest()
    {
        var connection = ConnectionMultiplexer.Connect(Const.Connection);
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
            StringArray = new[] { new KeyValuePair<string, string>("a12", "b"), new KeyValuePair<string, string>("c25", "d") },
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

        var re = RedisEntity<MyRecInt>.Default;
        Assert.That(ReferenceEquals(re.Fields, re.AllFields));
        Assert.That(ReferenceEquals(re.ReadFields, re.AllFields));
        Assert.That(ReferenceEquals(re.WriteFields, re.AllFields));
        Assert.That(re.Fields.Count, Is.EqualTo(1));
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

#if NETCOREAPP3_1_OR_GREATER
    [Test]
    public void DataAnnotationConfiguration_ReadKeyTest()
    {
        ReadKeyTest(new RedisEntityImpl<DocumentAnnotation>(
            new DataAnnotationConfiguration(
                RedisValueFormatterRegistry.Default)));
    }
#endif

    class ClassFieldsTest
    {
        private int _field;

        public int ReadOnlyField => _field;

        public int WriteOnlyField { set => _field = value; }
    }

    [Test]
    public void FieldsTest()
    {
        var fieldClass = new ClassFieldsTest { WriteOnlyField = 42 };

        var re = RedisEntity<ClassFieldsTest>.Default;

        Assert.That(re.Fields, Is.EqualTo(RedisEntityFields<ClassFieldsTest>.Empty));
        Assert.That(re.ReadFields.Count, Is.EqualTo(1));
        Assert.That(re.WriteFields.Count, Is.EqualTo(1));
        Assert.That(re.AllFields.Count, Is.EqualTo(2));

        Assert.That(re.ReadFields.ContainsKey(nameof(ClassFieldsTest.ReadOnlyField)), Is.True);
        Assert.That(re.WriteFields.ContainsKey(nameof(ClassFieldsTest.WriteOnlyField)), Is.True);

        var writeOnlyField = re.AllFields[nameof(ClassFieldsTest.WriteOnlyField)];
        Assert.That(writeOnlyField.CanWrite, Is.True);
        Assert.That(writeOnlyField.CanRead, Is.False);

        var readOnlyField = re.AllFields[nameof(ClassFieldsTest.ReadOnlyField)];
        Assert.That(readOnlyField.CanWrite, Is.False);
        Assert.That(readOnlyField.CanRead, Is.True);

        var key = "fieldClass";
        try
        {
            _db.EntitySet(key, fieldClass);
        }
        finally
        {
            _db.KeyDelete(key);
        }
    }

    [Test]
    public void ConfigurationBuilder_ReadKeyTest()
    {
        var configBuilder = new RedisEntityConfigurationBuilder(
            RedisValueFormatterRegistry.Default);

        configBuilder.Entity<DocumentAnnotation>().ConfigureDocumentAnnotation();
        configBuilder.Entity<IDocument>().ConfigureIDocument();

        var config = configBuilder.Build();
        var factory = new RedisEntityFactory(config);

        ReadKeyTest(factory.New<DocumentAnnotation>());

        ReadKey_Base64Formatter(factory.New<IDocument>());
    }

    private void ReadKeyTest(IRedisEntity<DocumentAnnotation> re)
    {
        var fields = re.Fields;
        var id = Guid.NewGuid();
        var doc = new DocumentAnnotation
        {
            Id = id,
            Name = "My Doc 1",
            AttachmentIds = new EquatableList<int> { 0, 1, 3, 5 }
        };
        var idempty = U8($"app:doc:{Guid.Empty:N}");
        var idbytes = U8($"app:doc:{id:N}");

        try
        {
            Assert.That(doc.RedisKeyBits, Is.EqualTo(1));
            Assert.That(doc.RedisKey, Is.Null);

            Assert.That(_db.EntitySet(doc, fields[nameof(DocumentAnnotation.AttachmentIds)], re), Is.True);

            Assert.That(doc.RedisKeyBits, Is.EqualTo(0));
            Assert.That(doc.RedisKey, Is.Not.Null);
            Assert.That(doc.RedisKey.SequenceEqual(idbytes), Is.True);

            var doc2 = new DocumentAnnotation();
            Assert.That(doc2.RedisKey, Is.Null);
            Assert.That(doc2.RedisKeyBits, Is.EqualTo(0));

            Assert.That(_db.EntityLoad(doc2, re), Is.False);

            Assert.That(doc2.RedisKeyBits, Is.EqualTo(0));
            Assert.That(doc2.RedisKey, Is.Not.Null);
            Assert.That(doc2.RedisKey.SequenceEqual(idempty), Is.True);
            doc2.Id = id;
            Assert.That(doc2.RedisKeyBits, Is.EqualTo(1));
            Assert.That(doc2.RedisKey.SequenceEqual(idempty), Is.True);

            Assert.That(_db.EntityLoad(doc2, re), Is.True);

            Assert.That(doc2.RedisKeyBits, Is.EqualTo(0));
            Assert.That(doc2.RedisKey.SequenceEqual(idbytes), Is.True);
            Assert.That(doc2.AttachmentIds, Is.EqualTo(doc.AttachmentIds));
            Assert.That(doc2.Name, Is.Null);

            Assert.That(_db.EntitySet(doc, re.Fields[nameof(DocumentAnnotation.Name)], re), Is.True);

            var doc3 = _db.EntityGet<DocumentAnnotation>(doc2.RedisKey, fields);

            Assert.That(doc3, Is.Not.Null);
            Assert.That(doc3.Id, Is.EqualTo(Guid.Empty));
            Assert.That(doc3.RedisKey, Is.Null);
            Assert.That(doc3.Name, Is.EqualTo(doc.Name));
            Assert.That(doc3.AttachmentIds, Is.EqualTo(doc.AttachmentIds));

            var redisKey = re.KeyBuilder.RebuildKey(null, 0, id);
            Assert.That(redisKey.SequenceEqual(idbytes), Is.True);
        }
        finally
        {
            if (doc.RedisKey != null)
                _db.KeyDelete(doc.RedisKey);
        }
    }

    private void ReadKey_Base64Formatter(IRedisEntity<IDocument> readerWriter)
    {
        var doc = new DocumentPOCO
        {
            Id = Guid.NewGuid(),
            Character = 'f',
            Name = "name",
        };

        var idbase64 = Convert.ToBase64String(doc.Id.ToByteArray()).Substring(0, 22);

        try
        {
            Assert.That(doc.RedisKey, Is.Null);
            doc.RedisKeyBits = 1;

            _db.EntitySet(doc, readerWriter);

            Assert.That(doc.RedisKey, Is.Not.Null);
            Assert.That(doc.RedisKey.SequenceEqual(U8("app:doc:" + idbase64)), Is.True);
            Assert.That(doc.RedisKeyBits, Is.EqualTo(0));
        }
        finally
        {
            if (doc.RedisKey != null)
                _db.KeyDelete(doc.RedisKey);
        }
    }

    private static byte[] U8(string str) => Encoding.UTF8.GetBytes(str);
}