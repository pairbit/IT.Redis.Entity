using IT.Redis.Entity.Internal;
using IT.Redis.Entity.Utf8Formatters;

namespace IT.Redis.Entity;

public class Utf8FormatterFixed : IUtf8Formatter
{
    public int GetLength<T>(in T value)
    {
        if (!Cache<T>.HasFormatter) throw Ex.Utf8FormatterNotFound(typeof(T));

        return Cache<T>.Formatter.GetLength(in value);
    }

    public bool TryFormat<T>(in T value, Span<byte> bytes, out int written)
    {
        if (!Cache<T>.HasFormatter) throw Ex.Utf8FormatterNotFound(typeof(T));

        return Cache<T>.Formatter.TryFormat(in value, bytes, out written);
    }

    static class Cache<T>
    {
        public static readonly IUtf8Formatter<T> Formatter = null!;
        public static readonly bool HasFormatter;

        static Cache()
        {
            var formatter = GetFormatter();
            if (formatter != null)
            {
                Formatter = formatter;
                HasFormatter = true;
            }
        }

        static IUtf8Formatter<T>? GetFormatter()
        {
            var type = typeof(T);
            if (type == typeof(Guid)) return (IUtf8Formatter<T>)GuidUtf8Formatter.Default;
            if (type == typeof(int)) return (IUtf8Formatter<T>)Int32Utf8Formatter.Fixed.L10;
            if (type == typeof(short)) return (IUtf8Formatter<T>)Int16Utf8Formatter.Fixed.L5;
            if (type == typeof(byte)) return (IUtf8Formatter<T>)ByteUtf8Formatter.Fixed.L3;

            return null;
        }
    }
}