using System.Runtime.Serialization;

namespace DocLib;

[DataContract]
public record Document
{
    public static readonly Document Empty = new();
    public static readonly Document Deleted = new() { IsDeleted = true };
    public static readonly Document Data = new()
    {
        Id = Guid.NewGuid(),
        Name = "Самый важный документ для сдачи проекта 2015",
        StartDate = new DateOnly(2020, 04, 22),
        Price = 274_620_500,
        Size = DocumentSize.Medium,
        Created = DateTime.UtcNow
    };

    public static readonly ReadOnlyDocument ReadOnlyData = new(Data);

    [DataMember(Order = 8)]
    public Guid Id { get; set; }

    [DataMember(Order = 0)]
    public string Name { get; set; } = null!;

    [DataMember(Order = 1)]
    public DateOnly StartDate { get; set; }

    [DataMember(Order = 2)]
    public DateOnly? EndDate { get; set; }

    [DataMember(Order = 3)]
    public long Price { get; set; }

    [DataMember(Order = 4)]
    public bool IsDeleted { get; set; }

    [DataMember(Order = 5)]
    public DocumentSize Size { get; set; }

    [DataMember(Order = 6)]
    public DateTime Created { get; set; }

    [DataMember(Order = 7)]
    public DateTime? Modified { get; set; }

    public static void New(Document doc, int i)
    {
        var random = Random.Shared;

        doc.Id = Guid.NewGuid();
        doc.Name = $"Самый важный документ для сдачи проекта №{i}";
        doc.StartDate = new DateOnly(random.Next(2000, 2024), random.Next(1, 13), random.Next(1, 29));
        doc.Price = random.NextInt64(1_000_000, 1_000_000_000);
        doc.Size = (DocumentSize)random.Next(0, 3);
        doc.Created = DateTime.UtcNow;
        doc.Modified = null;
        doc.EndDate = null;
        doc.IsDeleted = false;
    }
}