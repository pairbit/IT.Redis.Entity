using System.Reflection;
using System.Text;

namespace IT.Redis.Entity.Internal;

internal readonly record struct KeyInfo(PropertyInfo Property, object Utf8Formatter);

internal class EntityKeyBuilder : IKeyBuilder
{
    public static readonly int MaxKeys = 5;
    private static readonly char _separatorChar = ':';
    private static readonly byte _separator = (byte)_separatorChar;

    private readonly List<KeyInfo> _keys = new(MaxKeys);
    private readonly byte[] _prefix;
    private readonly Type _entityType;

    public EntityKeyBuilder(Type entityType, string? prefix)
    {
        _entityType = entityType;
        _prefix = GetPrefix(prefix);
    }

    public void AddKeyInfo(PropertyInfo property, object utf8Formatter)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (utf8Formatter == null) throw new ArgumentNullException(nameof(utf8Formatter));

        if (_keys.Count == MaxKeys) throw new InvalidOperationException($"A composite key containing more than {MaxKeys} fields is not supported");

        _keys.Add(new KeyInfo(property, utf8Formatter));
    }

    public byte[] BuildKey<TKey1>(byte[]? key, byte bits, in TKey1 key1)
    {
        var f1 = GetFormatter<TKey1>(0);
        var prefix = _prefix;
        var offset = prefix.Length;
        var length = offset + f1.GetLength(in key1);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            f1.Format(in key1, key.AsSpan(offset));
        }
        else if ((bits & 1) == 1)
        {
            f1.Format(in key1, key.AsSpan(offset));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var prefix = _prefix;
        var offset = prefix.Length;
        var lenKey1 = f1.GetLength(in key1);
        var length = 1 + offset + lenKey1 + f2.GetLength(in key2);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            offset += f1.Format(in key1, key.AsSpan(offset));
            key[offset++] = _separator;
            f2.Format(in key2, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));
            if ((bits & 2) == 2) f2.Format(in key2, key.AsSpan(offset + lenKey1 + 1));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var length = 2 + offset + lenKey1 + lenKey2 + f3.GetLength(in key3);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            offset += f1.Format(in key1, key.AsSpan(offset)); key[offset++] = sep;
            offset += f2.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            f3.Format(in key3, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f2.Format(in key2, key.AsSpan(offset));
            if ((bits & 4) == 4) f3.Format(in key3, key.AsSpan(offset + lenKey2 + 1));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var length = 3 + offset + lenKey1 + lenKey2 + lenKey3
                       + f4.GetLength(in key4);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            offset += f1.Format(in key1, key.AsSpan(offset)); key[offset++] = sep;
            offset += f2.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f3.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            f4.Format(in key4, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f2.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f3.Format(in key3, key.AsSpan(offset)); ;
            if ((bits & 8) == 8) f4.Format(in key4, key.AsSpan(offset + lenKey3 + 1));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var length = 4 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + f5.GetLength(in key5);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            offset += f1.Format(in key1, key.AsSpan(offset)); key[offset++] = sep;
            offset += f2.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f3.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f4.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            f5.Format(in key5, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f2.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f3.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f4.Format(in key4, key.AsSpan(offset));
            if ((bits & 16) == 16) f5.Format(in key5, key.AsSpan(offset + lenKey4 + 1));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var f6 = GetFormatter<TKey6>(5);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var lenKey5 = f5.GetLength(in key5);
        var length = 5 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + f6.GetLength(in key6);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            offset += f1.Format(in key1, key.AsSpan(offset)); key[offset++] = sep;
            offset += f2.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f3.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f4.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            offset += f5.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
            f6.Format(in key6, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f2.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f3.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f4.Format(in key4, key.AsSpan(offset));
            offset += lenKey4 + 1;
            if ((bits & 16) == 16) f5.Format(in key5, key.AsSpan(offset));
            if ((bits & 32) == 32) f6.Format(in key6, key.AsSpan(offset + lenKey5 + 1));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var f6 = GetFormatter<TKey6>(5);
        var f7 = GetFormatter<TKey7>(6);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var lenKey5 = f5.GetLength(in key5);
        var lenKey6 = f6.GetLength(in key6);
        var length = 6 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + lenKey6 + f7.GetLength(in key7);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            offset += f1.Format(in key1, key.AsSpan(offset)); key[offset++] = sep;
            offset += f2.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f3.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f4.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            offset += f5.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
            offset += f6.Format(in key6, key.AsSpan(offset)); key[offset++] = sep;
            f7.Format(in key7, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f2.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f3.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f4.Format(in key4, key.AsSpan(offset));
            offset += lenKey4 + 1;
            if ((bits & 16) == 16) f5.Format(in key5, key.AsSpan(offset));
            offset += lenKey5 + 1;
            if ((bits & 32) == 32) f6.Format(in key6, key.AsSpan(offset));
            if ((bits & 64) == 64) f7.Format(in key7, key.AsSpan(offset + lenKey6 + 1));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7, in TKey8 key8)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var f6 = GetFormatter<TKey6>(5);
        var f7 = GetFormatter<TKey7>(6);
        var f8 = GetFormatter<TKey8>(7);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var lenKey5 = f5.GetLength(in key5);
        var lenKey6 = f6.GetLength(in key6);
        var lenKey7 = f7.GetLength(in key7);
        var length = 7 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + lenKey6 + lenKey7 + f8.GetLength(in key8);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            offset += f1.Format(in key1, key.AsSpan(offset)); key[offset++] = sep;
            offset += f2.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f3.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f4.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            offset += f5.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
            offset += f6.Format(in key6, key.AsSpan(offset)); key[offset++] = sep;
            offset += f7.Format(in key7, key.AsSpan(offset)); key[offset++] = sep;
            f8.Format(in key8, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f2.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f3.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f4.Format(in key4, key.AsSpan(offset));
            offset += lenKey4 + 1;
            if ((bits & 16) == 16) f5.Format(in key5, key.AsSpan(offset));
            offset += lenKey5 + 1;
            if ((bits & 32) == 32) f6.Format(in key6, key.AsSpan(offset));
            offset += lenKey6 + 1;
            if ((bits & 64) == 64) f7.Format(in key7, key.AsSpan(offset));
            if ((bits & 128) == 128) f8.Format(in key8, key.AsSpan(offset + lenKey7 + 1));
        }
        return key;
    }

    private IUtf8Formatter<TKey> GetFormatter<TKey>(int index)
    {
        KeyInfo key;

        try
        {
            key = _keys[index];
        }
        catch (ArgumentOutOfRangeException)
        {
            throw Ex.InvalidKeyCount(_entityType, _keys);
        }

        try
        {
            return (IUtf8Formatter<TKey>)key.Utf8Formatter;
        }
        catch (InvalidCastException)
        {
            throw Ex.InvalidKeyType(typeof(TKey), key.Property);
        }
    }

    private static byte[] GetPrefix(string? prefix)
    {
        if (prefix == null) return Array.Empty<byte>();

        var encoding = Encoding.UTF8;
        var length = prefix.Length;

        if (prefix[length - 1] == _separatorChar)
            return encoding.GetBytes(prefix);

        var bytes = new byte[length + 1];

        encoding.GetBytes(prefix, bytes);

        bytes[length] = _separator;

        return bytes;
    }
}