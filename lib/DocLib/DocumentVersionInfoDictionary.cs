namespace DocLib;

public class DocumentVersionInfoDictionary : Dictionary<Guid, DocumentVersionInfo>
{
    public DocumentVersionInfoDictionary(int capacity) : base(capacity)
    {

    }
}