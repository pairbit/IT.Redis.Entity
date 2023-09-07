//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(StackExchange.Redis.Entity.Benchmarks.Benchmark));

using DocLib;
using DocLib.RedisEntity;
using StackExchange.Redis.Entity;

var doc = Document.Data;

RedisEntity<Document>.ReaderFactory = () => new RedisDocument();

var redisValue = RedisEntity<Document>.Reader.Read(doc, 0);

RedisEntity<Document>.ReaderFactory = () => new RedisDocumentArray();

redisValue = RedisEntity<Document>.Reader.Read(doc, 1);

RedisEntity<Document>.ReaderFactory = () => RedisEntity<Document>.Default;

redisValue = RedisEntity<Document>.Reader.Read(doc, 2);

Console.WriteLine(redisValue);