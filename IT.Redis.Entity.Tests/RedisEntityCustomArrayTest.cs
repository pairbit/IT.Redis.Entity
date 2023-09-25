using DocLib.RedisEntity;

namespace IT.Redis.Entity.Tests;

public class RedisEntityCustomArrayTest : RedisEntityTest
{
    private static readonly RedisDocumentArray ReaderWriter = new();

    public RedisEntityCustomArrayTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}