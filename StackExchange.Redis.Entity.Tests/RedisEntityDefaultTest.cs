using DocLib;

namespace StackExchange.Redis.Entity.Tests;

public class RedisEntityDefaultTest : RedisEntityTest
{
    private static readonly IRedisEntityReaderWriter<Document> ReaderWriter = RedisEntity<Document>.ReaderWriter;

    public RedisEntityDefaultTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}