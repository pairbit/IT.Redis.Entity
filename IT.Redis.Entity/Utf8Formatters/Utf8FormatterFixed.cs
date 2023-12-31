﻿using IT.Redis.Entity.Internal;

namespace IT.Redis.Entity.Utf8Formatters;

public class Utf8FormatterFixed : IUtf8Formatter
{
    public static readonly Utf8FormatterFixed Default = new();

    public int GetLength<T>(in T value)
        => Cache<T>.Formatter.GetLength(in value);

    public bool TryFormat<T>(in T value, Span<byte> bytes, out int written)
        => Cache<T>.Formatter.TryFormat(in value, bytes, out written);

    internal static object? GetFormatter(Type type)
    {
        if (type == typeof(Guid)) return GuidHexUtf8Formatter.Default;
        if (type == typeof(int)) return Int32Utf8Formatter.Fixed.L10;
        if (type == typeof(uint)) return UInt32Utf8Formatter.Fixed.L10;
        if (type == typeof(short)) return Int16Utf8Formatter.Fixed.L5;
        if (type == typeof(ushort)) return UInt16Utf8Formatter.Fixed.L5;
        if (type == typeof(byte)) return ByteUtf8Formatter.Fixed.L3;
        if (type == typeof(sbyte)) return SByteUtf8Formatter.Fixed.L3;
        if (type == typeof(byte[])) return ByteArrayUtf8Formatter.Default;
        if (type == typeof(string)) return StringUtf8Formatter.Default;

        return null;
    }

    static class Cache<T>
    {
        public static readonly IUtf8Formatter<T> Formatter =
            ((IUtf8Formatter<T>?)GetFormatter(typeof(T))) ??
            new Utf8FormatterNotFound<T>();
    }
}