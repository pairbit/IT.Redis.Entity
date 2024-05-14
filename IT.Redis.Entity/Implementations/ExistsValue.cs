namespace IT.Redis.Entity;

public readonly struct ExistsValue<T>
{
    private readonly bool _exists;
    private readonly T _value;

    public bool Exists => _exists;

    public T Value => _value;

    public ExistsValue(T value)
    {
        _exists = true;
        _value = value;
    }
}