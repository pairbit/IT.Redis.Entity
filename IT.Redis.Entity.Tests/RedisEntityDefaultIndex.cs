using DocLib;
using IT.Redis.Entity.Configurations;

namespace IT.Redis.Entity.Tests;

public class RedisEntityDefaultIndexTest : RedisEntityTest
{
    private static readonly IRedisEntityReaderWriter<Document> ReaderWriter 
        = new RedisEntityReaderWriterIndex<Document>(new DataContractConfiguration(RedisValueFormatterRegistry.Default));

    public RedisEntityDefaultIndexTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}