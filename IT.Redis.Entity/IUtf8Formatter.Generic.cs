namespace IT.Redis.Entity;

public interface IUtf8Formatter<T>
{
    int GetLength(in T value);

    bool TryFormat(in T value, Span<byte> span, out int written);
}