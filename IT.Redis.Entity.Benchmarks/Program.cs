using IT.Redis.Entity.Benchmarks;

//var listBench = new ListBenchmark();

//var sum = listBench.Array();
//if (listBench.LinkedListFor() != sum) throw new System.InvalidOperationException();
//if (listBench.LinkedListForeach() != sum) throw new System.InvalidOperationException();

//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(ListBenchmark));

//var bench = new RedisBenchmark();
//bench.KES();
//bench.KEB();
//bench.KE_String();
//bench.KE_Default();
//bench.KE_Fixed();

//var bench = new KeyBuilderBenchmark { Max = 3 };
//if (bench.Fixed_Manual() != bench.Fixed()) throw new System.Exception();
//BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(KeyBuilderBenchmark));

var bench = new Benchmark();
var e1 = bench.GetEntries();
var e2 = bench.GetEntries_FAST();
if (e1.Length != e2.Length) throw new System.InvalidOperationException();
for (int i = 0; i < e1.Length; i++)
{
    if (!e1[i].Equals(e2[i])) throw new System.InvalidOperationException();
}
BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(Benchmark));

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