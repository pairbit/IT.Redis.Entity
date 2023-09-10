namespace StackExchange.Redis.Entity.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class RedisFieldAttribute : Attribute
{
    public int Id { get; } = -1;

    public string? Name { get; }

    public RedisFieldAttribute(int id)
    {
        Id = id;
    }

    public RedisFieldAttribute(string name)
    {
        Name = name;
    }
}