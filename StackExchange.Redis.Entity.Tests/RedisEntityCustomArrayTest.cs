using DocLib.RedisEntity;

namespace StackExchange.Redis.Entity.Tests;

public class RedisEntityCustomArrayTest : RedisEntityTest
{
    private static readonly RedisDocumentReaderWriterArray ReaderWriter = new();

    public RedisEntityCustomArrayTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}