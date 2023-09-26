using DocLib;

namespace IT.Redis.Entity.Tests;

public class RedisEntityDefaultIndexTest : RedisEntityTest
{
    private static readonly IRedisEntityReaderWriter<Document> ReaderWriter 
        = new RedisEntityReaderWriterIndex<Document>(new RedisEntityConfiguration(RedisValueFormatterRegistry.Default));

    public RedisEntityDefaultIndexTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}