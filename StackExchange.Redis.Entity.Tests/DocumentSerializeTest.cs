using DocLib;
using MessagePack;
using MessagePack.Resolvers;

namespace StackExchange.Redis.Entity.Tests;

public class DocumentSerializeTest
{
    [Test]
    public void SerializeContractless()
    {
        Assert.That(DocumentEqualityComparer.Default.Equals(new DocumentPOCO(), new DocumentPOCO()), Is.True);

        var doc = DocumentGenerator.New<DocumentPOCO>();

        var bytes = MessagePackSerializer.Serialize(doc, ContractlessStandardResolver.Options);

        var json = MessagePackSerializer.ConvertToJson(bytes);

        var doc2 = MessagePackSerializer.Deserialize<DocumentPOCO>(bytes, ContractlessStandardResolver.Options);

        doc.VersionInfo = null;//IGNORE
        Assert.That(ReferenceEquals(doc, doc2), Is.False);
        Assert.That(DocumentEqualityComparer.Default.Equals(doc, doc2), Is.True);
    }

    [Test]
    public void SerializeContract()
    {
        Assert.That(DocumentEqualityComparer.Default.Equals(new DocumentDataContract(), new DocumentDataContract()), Is.True);

        var doc = new DocumentDataContract();
        
        for (int i = 0; i < 100; i++)
        {
            DocumentGenerator.Next(doc, i);

            var bytes = MessagePackSerializer.Serialize(doc);

            var json = MessagePackSerializer.ConvertToJson(bytes);

            var doc2 = MessagePackSerializer.Deserialize<DocumentDataContract>(bytes);

            doc.VersionInfo = null;//IGNORE
            Assert.That(DocumentEqualityComparer.Default.Equals(doc, doc2), Is.True);
        }
    }
}