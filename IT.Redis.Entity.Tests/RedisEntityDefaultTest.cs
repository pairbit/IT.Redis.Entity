using DocLib;

namespace IT.Redis.Entity.Tests;

public class RedisEntityDefaultTest : RedisEntityTest
{
    public RedisEntityDefaultTest() : base(RedisEntity<Document>.Default)
    {
    }
}