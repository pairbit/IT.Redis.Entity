namespace IT.Redis.Entity.Tests;

public class Int64MaxValueTest
{
    [Test]
    public void Test()
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
}