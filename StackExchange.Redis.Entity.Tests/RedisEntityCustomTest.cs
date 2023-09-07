using DocLib.RedisEntity;

namespace StackExchange.Redis.Entity.Tests;

public class RedisEntityCustomTest : RedisEntityTest
{
    private static readonly RedisDocumentReaderWriter ReaderWriter = new();

    public RedisEntityCustomTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}