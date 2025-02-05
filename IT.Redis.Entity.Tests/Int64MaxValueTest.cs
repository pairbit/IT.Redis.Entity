namespace IT.Redis.Entity.Tests;

public class Int64MaxValueTest
{
    [Test]
    public void Int64Test()
    {
        var db = Shared.Db;

        Assert.That(db.HashSet("Int64", "max", Int64.MaxValue), Is.True);
        Assert.That(db.HashSet("Int64", "min", Int64.MinValue), Is.True);

        try
        {
            Assert.That(
                Assert.Throws<RedisServerException>(() => db.HashIncrement("Int64", "max", 1))!.Message,
                Is.EqualTo("ERR increment or decrement would overflow"));

            Assert.That(
                Assert.Throws<RedisServerException>(() => db.HashDecrement("Int64", "min", 1))!.Message,
                Is.EqualTo("ERR increment or decrement would overflow"));
        }
        finally
        {
            db.KeyDelete("Int64");
        }
    }

    [Test]
    public void UInt64Test()
    {
        var db = Shared.Db;

        Assert.That(db.HashSet("UInt64", "max", (UInt64)UInt64.MaxValue), Is.True);

        try
        {
            Assert.That(
                Assert.Throws<RedisServerException>(() => db.HashIncrement("UInt64", "max", 1))!.Message,
                Is.EqualTo("ERR hash value is not an integer"));
        }
        finally
        {
            db.KeyDelete("UInt64");
        }
    }
}