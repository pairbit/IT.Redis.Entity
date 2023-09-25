using IT.Redis.Entity.Benchmarks;

var bench = new Benchmark();
BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(IT.Redis.Entity.Benchmarks.Benchmark));

//using DocLib;
//using DocLib.RedisEntity;
//using IT.Redis.Entity;

//var doc = Document.Data;

//RedisEntity<Document>.ReaderFactory = () => new RedisDocument();

//var redisValue = RedisEntity<Document>.Reader.Read(doc, 0);

//RedisEntity<Document>.ReaderFactory = () => new RedisDocumentArray();

//redisValue = RedisEntity<Document>.Reader.Read(doc, 1);

//RedisEntity<Document>.ReaderFactory = () => RedisEntity<Document>.Default;

//redisValue = RedisEntity<Document>.Reader.Read(doc, 2);

//Console.WriteLine(redisValue);