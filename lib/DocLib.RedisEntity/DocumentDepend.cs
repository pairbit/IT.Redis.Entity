using StackExchange.Redis.Entity.Attributes;

namespace DocLib.RedisEntity;

public class DocumentDepend
{
    [GuidHexFormatter("n")]
    public Guid Id { get; set; }

    [GuidHexFormatter]
    public Guid ClientId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public long Price { get; set; }

    public bool IsDeleted { get; set; }

    public DocumentSize Size { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }
}
