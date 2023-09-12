namespace DocLib;

public static class DocumentGenerator
{
    public static readonly DocumentPOCO Empty = new();
    public static readonly DocumentPOCO Deleted = new() { IsDeleted = true };
    public static readonly DocumentPOCO Data = new()
    {
        Id = Guid.NewGuid(),
        Name = "Самый важный документ для сдачи проекта 2015",
        StartDate = new DateOnly(2020, 04, 22),
        Price = 274_620_500,
        Size = DocumentSize.Medium,
        Created = DateTime.UtcNow
    };

    public static T New<T>(int i = 0) where T : IDocument, new()
    {
        var doc = new T();
        Next(doc, i);
        return doc;
    }

    public static void Next(IDocument doc, int i)
    {
        var random = Random.Shared;

        doc.Id = Guid.NewGuid();
        doc.ExternalId = Guid.NewGuid();
        doc.Name = $"Самый важный документ для сдачи проекта №{i}";
        doc.Url = new Uri("https://www.youtube.com/");
        doc.Version = new Version(12, 3, 23332, 33);
        doc.StartDate = new DateOnly(random.Next(2000, 2024), random.Next(1, 13), random.Next(1, 29));
        doc.EndDate = new DateOnly(random.Next(2010, 2030), random.Next(1, 13), random.Next(1, 29));
        doc.Price = random.NextInt64(1_000_000, 1_000_000_000);
        doc.Size = (DocumentSize)random.Next(0, 3);
        doc.Created = DateTime.UtcNow;
        doc.Modified = null;
        doc.IsDeleted = false;
        var bytes = new byte[32];
        random.NextBytes(bytes);
        doc.BigInteger = new System.Numerics.BigInteger(bytes);
        doc.Character = (char)random.Next(0, 60000);
        bytes = new byte[1024];
        random.NextBytes(bytes);
        doc.Content = bytes;
        bytes = new byte[4024];
        random.NextBytes(bytes);
        doc.MemoryBytes = bytes;
        doc.Bits = new System.Collections.BitArray(new int[2] { random.Next(), random.Next() });
        doc.IntArray = new int[] { random.Next(), random.Next(), random.Next(), random.Next() };
        doc.IntArrayN = new int?[] { random.Next(), null, random.Next(), null, null };
        doc.TagIds = new List<Guid?>() { Guid.NewGuid(), null, Guid.NewGuid(), Guid.NewGuid(), null };
    }
}