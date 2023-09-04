using System.Runtime.Serialization;

namespace ExternalLib;

[DataContract]
public record Document
{
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
}