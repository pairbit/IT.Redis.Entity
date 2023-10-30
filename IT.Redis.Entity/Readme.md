# IT.Redis.Entity
[![NuGet version (IT.Redis.Entity)](https://img.shields.io/nuget/v/IT.Redis.Entity.svg)](https://www.nuget.org/packages/IT.Redis.Entity)
[![NuGet pre version (IT.Redis.Entity)](https://img.shields.io/nuget/vpre/IT.Redis.Entity.svg)](https://www.nuget.org/packages/IT.Redis.Entity)

Object mapping for Redis

## Entity class declaration

```csharp
    class Person1
    {
        private readonly int _id;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Field '_redisKey' is never assigned to, and will always have its default value null
        private byte[]? _redisKey;
#pragma warning restore CS0649 // Field '_redisKey' is never assigned to, and will always have its default value null
#pragma warning restore IDE0044 // Add readonly modifier

        public byte[]? RedisKey => _redisKey;

        public Person1(int id) => _id = id;

        public int Id => _id;

        public string? Name { get; set; }
    }
```

## Init RedisValue formatter

```csharp
#if NETCOREAPP3_1_OR_GREATER
    var formatter = IT.Redis.Entity.Formatters.JsonFormatter.Default;
#endif
```

## Set up a factory

```csharp
var configBuilder = new RedisEntityConfigurationBuilder(formatter);

configBuilder
.HasKeyPrefix<Person1>("app:persons")
.HasKey<Person1, int>(x => x.Id);

var factory = new RedisEntityFactory(configBuilder.Build());
```

## Connect to the database redis

```csharp
var connection = ConnectionMultiplexer.Connect("localhost:6379,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");

var db = connection.GetDatabase()!;
```

## New Person1 and save to redis

```csharp
var readerWriter = factory.NewReaderWriter<Person1>();

var person = new Person1(12) { Name = "John" };

db.EntitySet(person, readerWriter);

db.KeyDelete(person.RedisKey);
```

## Set global factory

```csharp
RedisEntity.Factory = factory;
```