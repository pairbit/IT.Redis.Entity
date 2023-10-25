using IT.Redis.Entity.Formatters.Utf8;

namespace IT.Redis.Entity;

public class Utf8FormatterDefault : IUtf8Formatter
{
    public int GetLength<T>(in T value)
        => Cache<T>.Formatter.GetLength(in value);

    public bool TryFormat<T>(in T value, Span<byte> bytes, out int written)
        => Cache<T>.Formatter.TryFormat(in value, bytes, out written);

    static class Cache<T>
    {
        public static readonly IUtf8Formatter<T> Formatter = GetFormatter();

        static IUtf8Formatter<T> GetFormatter()
        {
            if (typeof(T) == typeof(Guid)) return (IUtf8Formatter<T>)GuidUtf8Formatter.Default;
            if (typeof(T) == typeof(string)) return (IUtf8Formatter<T>)StringUtf8Formatter.Default;

            throw new InvalidOperationException();
        }
    }
}