# IT.Redis.Entity
[![NuGet version (IT.Redis.Entity)](https://img.shields.io/nuget/v/IT.Redis.Entity.svg)](https://www.nuget.org/packages/IT.Redis.Entity)
[![NuGet pre version (IT.Redis.Entity)](https://img.shields.io/nuget/vpre/IT.Redis.Entity.svg)](https://www.nuget.org/packages/IT.Redis.Entity)
[![GitHub Actions](https://github.com/pairbit/IT.Redis.Entity/workflows/Build/badge.svg)](https://github.com/pairbit/IT.Redis.Entity/actions)
[![Releases](https://img.shields.io/github/release/pairbit/IT.Redis.Entity.svg)](https://github.com/pairbit/IT.Redis.Entity/releases)

Object mapping for Redis

## Declaring an entity class with mutable keys

```csharp
    class Document
    {
        private Guid _guid;
        private int _index;

        // Add readonly modifier
#pragma warning disable IDE0044
#pragma warning disable CS0649
        private byte[]? _redisKey;
#pragma warning restore CS0649
#pragma warning restore IDE0044

        private byte _redisKeyBits;

        public byte[]? RedisKey => _redisKey;

        public byte RedisKeyBits => _redisKeyBits;

        public Guid Guid
        {
            get => _guid;
            set
            {
                if (_guid != value)
                {
                    _guid = value;
                    _redisKeyBits |= 1;
                }
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    _redisKeyBits |= 2;
                }
            }
        }

        public string? Name { get; set; }
    }
```

## Init RedisValue formatter

```csharp
#if NETCOREAPP3_1_OR_GREATER
    var formatter = IT.Redis.Entity.Formatters.JsonFormatter.Default;
#endif
```

## Configuration

```csharp
var configBuilder = new RedisEntityConfigurationBuilder(formatter);

configBuilder
.Entity<Document>()
.HasKeyPrefix("app:docs")
.HasKey(x => x.Guid)
.HasKey(x => x.Index);

var config = configBuilder.Build();

var reDoc = config.NewEntity<Document>();
```

## Connect to the database redis

```csharp
var connection = ConnectionMultiplexer.Connect(connectionString);

var db = connection.GetDatabase()!;
```

## New Document and save to redis

```csharp
var guid = Guid.NewGuid();
var doc = new Document
{
    Guid = guid,
    Index = 1,
    Name = "doc1"
};

db.EntitySet(doc, reDoc);

doc.Index = 2;
doc.Name = "doc2";

db.EntitySet(doc, reDoc);
```

## Load into existing Document

```csharp
doc.Index = 3;
Assert.That(db.EntityLoad(doc, reDoc), Is.False);
Assert.That(doc.Name, Is.EqualTo("doc2"));

doc.Index = 1;
Assert.That(db.EntityLoad(doc, reDoc), Is.True);
Assert.That(doc.Name, Is.EqualTo("doc1"));
```

## Build redisKey from EntityKeyBuilder

```csharp
var redisKey = reDoc.KeyBuilder.BuildKey(guid, 1);
Assert.That(redisKey, Is.EqualTo(doc.RedisKey));
```

## Build redisKey from KeyBuilder

```csharp
var redisKey2 = KeyBuilder.Default.BuildKey("app:docs", guid, 1);
Assert.That(redisKey2, Is.EqualTo(redisKey));
```

## Get Document by redis key

```csharp
var doc1 = db.EntityGet(redisKey, reDoc);
Assert.That(doc1, Is.Not.Null);
Assert.That(doc1.Name, Is.EqualTo("doc1"));

var redisKey3 = reDoc.KeyBuilder.BuildKey(guid, 3);
var doc3 = db.EntityGet(redisKey3, reDoc);
Assert.That(doc3, Is.Null);
```

## Set global factories

```csharp
RedisEntity<Document>.Factory = () => reDoc;

RedisEntity.Config = config;
```
