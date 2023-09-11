using DocLib;
using MessagePack;
using MessagePack.Resolvers;

namespace StackExchange.Redis.Entity.Tests;

public class DocumentSerializeTest
{
    [Test]
    public void Serialize()
    {
        Assert.That(DocumentEqualityComparer.Default.Equals(new DocumentPOCO(), new DocumentPOCO()), Is.True);

        var doc = DocumentGenerator.New<DocumentPOCO>();

        var bytes = MessagePackSerializer.Serialize(doc, ContractlessStandardResolver.Options);

        var json = MessagePackSerializer.ConvertToJson(bytes);

        var doc2 = MessagePackSerializer.Deserialize<DocumentPOCO>(bytes, ContractlessStandardResolver.Options);

        Assert.That(ReferenceEquals(doc, doc2), Is.False);
        Assert.That(DocumentEqualityComparer.Default.Equals(doc, doc2), Is.True);

        var docSeri = new DocumentDataContract();
        for (int i = 0; i < 100; i++)
        {
            DocumentGenerator.Next(docSeri, i);

            bytes = MessagePackSerializer.Serialize(docSeri);

            json = MessagePackSerializer.ConvertToJson(bytes);

            var docSeri2 = MessagePackSerializer.Deserialize<DocumentDataContract>(bytes);

            Assert.That(DocumentEqualityComparer.Default.Equals(docSeri, docSeri2), Is.True);
        }
    }
}