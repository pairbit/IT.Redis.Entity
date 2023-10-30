using IT.Redis.Entity.Internal;
using System.Buffers.Text;
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
            KeyBuilder.Default.BuildKey(null, 0, default(MyTypeId))).Message,
            Is.EqualTo(exception.Message));

        Assert.That(Assert.Throws<ArgumentException>(() =>
            KeyBuilder.Fixed.BuildKey(null, 0, default(MyTypeId))).Message,
            Is.EqualTo(exception.Message));
    }

    [Test]
    public void Key1Test()
    {
        var builder = KeyBuilder.Default;

        var id = Guid.NewGuid();
        var idb = U8(id.ToString("N"));
        var empty = new byte[idb.Length];

        Assert.That(builder.BuildKey(empty, 0, id) == empty, Is.True);
        Assert.That(builder.BuildKey(idb, 0, id) == idb, Is.True);

        Assert.That(builder.BuildKey(null, 0, id).SequenceEqual(idb), Is.True);
        Assert.That(builder.BuildKey(Array.Empty<byte>(), 0, id).SequenceEqual(idb), Is.True);
        Assert.That(builder.BuildKey(new byte[idb.Length - 1], 0, id).SequenceEqual(idb), Is.True);
        Assert.That(builder.BuildKey(new byte[idb.Length + 1], 0, id).SequenceEqual(idb), Is.True);

        var key = builder.BuildKey(null, 1, id);
        Assert.That(key.SequenceEqual(idb), Is.True);
        Assert.That(builder.BuildKey(key, 0, id) == key, Is.True);
        Assert.That(builder.BuildKey(key, 1, id) == key, Is.True);

        id = Guid.NewGuid();
        Assert.That(builder.BuildKey(key, 1, id) == key, Is.True);
        Assert.That(key.SequenceEqual(idb), Is.False);
        Assert.That(key.SequenceEqual(U8(id.ToString("N"))), Is.True);
    }

    [Test]
    public void Key2Test()
    {
        var builder = KeyBuilder.Default;

        var id = Guid.NewGuid();
        var prefix = U8("prefix");
        var pid = U8($"prefix:{id:N}");
        var empty = new byte[pid.Length];

        Assert.That(builder.BuildKey(empty, 0, prefix, id) == empty, Is.True);
        Assert.That(builder.BuildKey(pid, 0, prefix, id) == pid, Is.True);

        Assert.That(builder.BuildKey(null, 0, prefix, id).SequenceEqual(pid), Is.True);
        Assert.That(builder.BuildKey(Array.Empty<byte>(), 0, prefix, id).SequenceEqual(pid), Is.True);
        Assert.That(builder.BuildKey(new byte[pid.Length - 1], 0, prefix, id).SequenceEqual(pid), Is.True);
        Assert.That(builder.BuildKey(new byte[pid.Length + 1], 0, prefix, id).SequenceEqual(pid), Is.True);

        var key = builder.BuildKey(null, 3, prefix, id);
        Assert.That(key.SequenceEqual(pid), Is.True);
        Assert.That(builder.BuildKey(key, 0, prefix, id) == key, Is.True);
        Assert.That(builder.BuildKey(key, 1, prefix, id) == key, Is.True);
        Assert.That(builder.BuildKey(key, 2, prefix, id) == key, Is.True);
        Assert.That(builder.BuildKey(key, 3, prefix, id) == key, Is.True);

        //change only prefix
        prefix = U8("PREFIX");
        Assert.That(builder.BuildKey(key, 1, prefix, id) == key, Is.True);
        Assert.That(key.SequenceEqual(pid), Is.False);
        Assert.That(key.SequenceEqual(U8($"PREFIX:{id:N}")), Is.True);

        //change only id
        id = Guid.NewGuid();
        Assert.That(builder.BuildKey(key, 2, prefix, id) == key, Is.True);
        Assert.That(key.SequenceEqual(pid), Is.False);
        Assert.That(key.SequenceEqual(U8($"PREFIX:{id:N}")), Is.True);
    }
    
    [Test]
    public void Key8Test()
    {
        var builder = KeyBuilder.Default;

        var key = builder.BuildKey(null, 0, 1, 2, 3, 4, 5, 6, 7, 8);

        Assert.That(key.SequenceEqual(U8("1:2:3:4:5:6:7:8")), Is.True);

        Assert.That(builder.BuildKey(key, 255, 8, 7, 6, 5, 4, 3, 2, 1) == key, Is.True);

        Assert.That(key.SequenceEqual(U8("8:7:6:5:4:3:2:1")), Is.True);
    }

    [Test]
    public void KeysTest()
    {
        var builder = KeyBuilder.Default;

        Assert.That(builder
            .BuildKey(null, 255, 1)
            .SequenceEqual(U8("1")), Is.True);

        Assert.That(builder
            .BuildKey(null, 255, 1, 2)
            .SequenceEqual(U8("1:2")), Is.True);

        Assert.That(builder
            .BuildKey(null, 255, 1, 2, 3)
            .SequenceEqual(U8("1:2:3")), Is.True);

        Assert.That(builder
            .BuildKey(null, 255, 1, 2, 3, 4)
            .SequenceEqual(U8("1:2:3:4")), Is.True);

        Assert.That(builder
            .BuildKey(null, 255, 1, 2, 3, 4, 5)
            .SequenceEqual(U8("1:2:3:4:5")), Is.True);

        Assert.That(builder
            .BuildKey(null, 255, 1, 2, 3, 4, 5, 6)
            .SequenceEqual(U8("1:2:3:4:5:6")), Is.True);

        Assert.That(builder
            .BuildKey(null, 255, 1, 2, 3, 4, 5, 6, 7)
            .SequenceEqual(U8("1:2:3:4:5:6:7")), Is.True);

        Assert.That(builder
            .BuildKey(null, 255, 1, 2, 3, 4, 5, 6, 7, 8)
            .SequenceEqual(U8("1:2:3:4:5:6:7:8")), Is.True);
    }

    [Test]
    public void AllocTest()
    {
        AllocForByteTest(KeyBuilder.Default, 2);
        AllocForByteTest(KeyBuilder.Fixed, 0, "d3");
    }

    [Test]
    public void RedisKeyModifyTest()
    {
        var builder = KeyBuilder.Default;
        var prefix = U8("prefix");

        var key = builder.BuildKey(null, 0, prefix, (sbyte)0);

        RedisKey redisKey = key;
        RedisKey redisKey2 = "prefix:0";

        Assert.That(redisKey.Equals(redisKey2), Is.True);

        builder.BuildKey(key, 2, prefix, (sbyte)1);

        Assert.That(redisKey.Equals("prefix:1"), Is.True);
    }

    [Test]
    public void KeyFixed()
    {
        var builder = KeyBuilder.Fixed;

        Assert.That(builder.BuildKey(null, 1, 0).SequenceEqual(U8("0000000000")), Is.True);
        Assert.That(builder.BuildKey(null, 1, int.MaxValue).SequenceEqual(U8("2147483647")), Is.True);
        Assert.That(builder.BuildKey(null, 1, uint.MaxValue).SequenceEqual(U8("4294967295")), Is.True);

        Assert.That(builder.BuildKey(null, 1, (short)0).SequenceEqual(U8("00000")), Is.True);
        Assert.That(builder.BuildKey(null, 1, short.MaxValue).SequenceEqual(U8("32767")), Is.True);
        Assert.That(builder.BuildKey(null, 1, ushort.MaxValue).SequenceEqual(U8("65535")), Is.True);

        Assert.That(builder.BuildKey(null, 1, (byte)0).SequenceEqual(U8("000")), Is.True);
        Assert.That(builder.BuildKey(null, 1, byte.MaxValue).SequenceEqual(U8("255")), Is.True);
        Assert.That(builder.BuildKey(null, 1, sbyte.MaxValue).SequenceEqual(U8("127")), Is.True);
    }

    [Test]
    public void IntFormatD()
    {
        var i = int.MaxValue;
        var bytes = new byte[10];
        var format = "d" + bytes.Length;
        var standardFormat = System.Buffers.StandardFormat.Parse(format);
        var str = i.ToString(format);
        Utf8Formatter.TryFormat(i, bytes, out _, standardFormat);
        Assert.That(bytes.SequenceEqual(U8(str)), Is.True);
    }

    [Test]
    public void BitsTest()
    {
        byte bits = 0;
        Assert.That(bits |= 1 << 0, Is.EqualTo(1));
        Assert.That(bits |= 1 << 1, Is.EqualTo(3));
        Assert.That(bits |= 1 << 2, Is.EqualTo(7));
        Assert.That(bits |= 1 << 3, Is.EqualTo(15));
        Assert.That(bits |= 1 << 4, Is.EqualTo(31));
        Assert.That(bits |= 1 << 5, Is.EqualTo(63));
        Assert.That(bits |= 1 << 6, Is.EqualTo(127));
        Assert.That(bits |= 1 << 7, Is.EqualTo(255));

        Assert.That(bits = 0, Is.EqualTo(0));
        Assert.That(bits |= 1, Is.EqualTo(1)); Assert.That(bits & 1, Is.EqualTo(1));
        Assert.That(bits |= 2, Is.EqualTo(3)); Assert.That(bits & 2, Is.EqualTo(2));
        Assert.That(bits |= 4, Is.EqualTo(7)); Assert.That(bits & 4, Is.EqualTo(4));
        Assert.That(bits |= 8, Is.EqualTo(15)); Assert.That(bits & 8, Is.EqualTo(8));
        Assert.That(bits |= 16, Is.EqualTo(31)); Assert.That(bits & 16, Is.EqualTo(16));
        Assert.That(bits |= 32, Is.EqualTo(63)); Assert.That(bits & 32, Is.EqualTo(32));
        Assert.That(bits |= 64, Is.EqualTo(127)); Assert.That(bits & 64, Is.EqualTo(64));
        Assert.That(bits |= 128, Is.EqualTo(255)); Assert.That(bits & 128, Is.EqualTo(128));
    }

    private void AllocForByteTest(IKeyBuilder builder, int allocations, string? numFormat = null)
    {
        var prefix = U8("prefix");
        var key = builder.BuildKey(null, 0, prefix, (byte)0);

        Assert.That(key.SequenceEqual(U8("prefix:" + (0).ToString(numFormat))), Is.True);

        var alloc = 0;

        for (byte i = 1; i < 255; i++)
        {
            var newKey = builder.BuildKey(key, 2, prefix, i);

            Assert.That(newKey.SequenceEqual(U8($"prefix:{i.ToString(numFormat)}")), Is.True);

            if (newKey != key)
            {
                alloc++;
                key = newKey;
            }
        }

        Assert.That(alloc, Is.EqualTo(allocations));
    }

    private static byte[] U8(string str) => Encoding.UTF8.GetBytes(str);
}