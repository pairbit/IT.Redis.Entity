using System.Runtime.InteropServices;

namespace DocLib;

[StructLayout(LayoutKind.Explicit)]
public readonly record struct DocumentVersionInfo
{
    [FieldOffset(0)] private readonly Guid _id;
    [FieldOffset(16)] private readonly Guid _authodId;
    [FieldOffset(32)] private readonly DateTime _date;
    [FieldOffset(40)] private readonly long _number;

    public DocumentVersionInfo(Guid id, Guid authodId, DateTime date, long number)
    {
        _id = id;
        _authodId = authodId;
        _date = date;
        _number = number;
    }

    public Guid Id => _id;

    public DateTime Date => _date;

    public Guid AuthodId => _authodId;

    public long Number => _number;
}