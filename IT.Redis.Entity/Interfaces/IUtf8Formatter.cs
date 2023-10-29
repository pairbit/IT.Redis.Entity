namespace IT.Redis.Entity;

public interface IUtf8Formatter
{
    int GetLength<T>(in T value);

    bool TryFormat<T>(in T value, Span<byte> utf8, out int written);
}