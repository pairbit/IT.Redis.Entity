using System.Runtime.Serialization;

namespace DocLib;

[DataContract]
public readonly record struct DocumentVersionInfo
{
    private readonly Guid _id;
    private readonly Guid _authodId;
    private readonly DateTime _date;
    private readonly long _number;

    public DocumentVersionInfo(Guid id, Guid authodId, DateTime date, long number)
    {
        _id = id;
        _authodId = authodId;
        _date = date;
        _number = number;
    }

    [DataMember(Order = 0)]
    public Guid Id => _id;

    [DataMember(Order = 1)]
    public DateTime Date => _date;

    [DataMember(Order = 2)]
    public Guid AuthodId => _authodId;

    [DataMember(Order = 3)]
    public long Number => _number;
}