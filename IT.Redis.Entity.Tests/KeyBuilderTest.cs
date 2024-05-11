using IT.Redis.Entity.Internal;
using System.Text;

namespace IT.Redis.Entity.Tests;

public class KeyBuilderTest
{
    struct MyTypeId { }

    [Test]
    public void Utf8FormatterNotFoundTest()
    {
        var exception = Ex.Utf8FormatterNotFound(typeof(MyTypeId));

        Assert.That(Assert.Throws<ArgumentException>(() =>
            KeyBuilder.Default.BuildKey(default(MyTypeId))).Message,
            Is.EqualTo(exception.Message));

        Assert.That(Assert.Throws<ArgumentException>(() =>
            KeyBuilder.Fixed.BuildKey(default(MyTypeId))).Message,
            Is.EqualTo(exception.Message));
    }

    [Test]
    public void Key1Test()
    {
        var builder = KeyBuilder.Default;

        var id = Guid.NewGuid();
        var idb = U8(id.ToString("N"));

        Assert.That(builder.BuildKey(id).SequenceEqual(idb), Is.True);
    }

    [Test]
    public void Key2Test()
    {
        var builder = KeyBuilder.Default;

        var id = Guid.NewGuid();
        var prefix = U8("prefix");
        var pid = U8($"prefix:{id:N}");

        Assert.That(builder.BuildKey(prefix, id).SequenceEqual(pid), Is.True);
    }

    [Test]
    public void Key8Test()
    {
        var builder = KeyBuilder.Default;

        var key = builder.BuildKey(1, 2, 3, 4, 5, 6, 7, 8);

        Assert.That(key.SequenceEqual(U8("1:2:3:4:5:6:7:8")), Is.True);
    }

    [Test]
    public void KeysTest()
    {
        var builder = KeyBuilder.Default;

        Assert.That(builder
            .BuildKey(1)
            .SequenceEqual(U8("1")), Is.True);

        Assert.That(builder
            .BuildKey(1, 2)
            .SequenceEqual(U8("1:2")), Is.True);

        Assert.That(builder
            .BuildKey(1, 2, 3)
            .SequenceEqual(U8("1:2:3")), Is.True);

        Assert.That(builder
            .BuildKey(1, 2, 3, 4)
            .SequenceEqual(U8("1:2:3:4")), Is.True);

        Assert.That(builder
            .BuildKey(1, 2, 3, 4, 5)
            .SequenceEqual(U8("1:2:3:4:5")), Is.True);

        Assert.That(builder
            .BuildKey(1, 2, 3, 4, 5, 6)
            .SequenceEqual(U8("1:2:3:4:5:6")), Is.True);

        Assert.That(builder
            .BuildKey(1, 2, 3, 4, 5, 6, 7)
            .SequenceEqual(U8("1:2:3:4:5:6:7")), Is.True);

        Assert.That(builder
            .BuildKey(1, 2, 3, 4, 5, 6, 7, 8)
            .SequenceEqual(U8("1:2:3:4:5:6:7:8")), Is.True);
    }

    [Test]
    public void KeyFixed()
    {
        var builder = KeyBuilder.Fixed;

        Assert.That(builder.BuildKey(0).SequenceEqual(U8("0000000000")), Is.True);
        Assert.That(builder.BuildKey(int.MaxValue).SequenceEqual(U8("2147483647")), Is.True);
        Assert.That(builder.BuildKey(uint.MaxValue).SequenceEqual(U8("4294967295")), Is.True);

        Assert.That(builder.BuildKey((short)0).SequenceEqual(U8("00000")), Is.True);
        Assert.That(builder.BuildKey(short.MaxValue).SequenceEqual(U8("32767")), Is.True);
        Assert.That(builder.BuildKey(ushort.MaxValue).SequenceEqual(U8("65535")), Is.True);

        Assert.That(builder.BuildKey((byte)0).SequenceEqual(U8("000")), Is.True);
        Assert.That(builder.BuildKey(byte.MaxValue).SequenceEqual(U8("255")), Is.True);
        Assert.That(builder.BuildKey(sbyte.MaxValue).SequenceEqual(U8("127")), Is.True);
    }

    private static byte[] U8(string str) => Encoding.UTF8.GetBytes(str);
}