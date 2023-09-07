using DocLib.RedisEntity;

namespace StackExchange.Redis.Entity.Tests;

public class RedisEntityCustomArrayExpressionTest : RedisEntityTest
{
    private static readonly RedisDocumentArrayExpression ReaderWriter = new();

    public RedisEntityCustomArrayExpressionTest() : base(ReaderWriter, ReaderWriter)
    {

    }
}