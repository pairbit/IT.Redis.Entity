namespace IT.Redis.Entity;

public interface IFixSerializer<T>
{
    int GetSerializedLength(in T value);

    int Serialize(in T value, Span<byte> span);
}