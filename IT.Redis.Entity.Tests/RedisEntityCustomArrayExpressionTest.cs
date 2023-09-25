using DocLib.RedisEntity;

namespace IT.Redis.Entity.Tests;

public class RedisEntityCustomArrayExpressionTest : RedisEntityTest
{
    private static readonly RedisDocumentArrayExpression ReaderWriter = new();

    public RedisEntityCustomArrayExpressionTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}