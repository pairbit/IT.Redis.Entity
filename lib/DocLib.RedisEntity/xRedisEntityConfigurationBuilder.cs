using IT.Redis.Entity.Configurations;
using IT.Redis.Entity.Utf8Formatters;

namespace DocLib.RedisEntity;

public static class xRedisEntityConfigurationBuilder
{
    public static RedisEntityConfigurationBuilder<DocumentAnnotation> ConfigureDocumentAnnotation(this RedisEntityConfigurationBuilder<DocumentAnnotation> builder)
    {
        return builder
               .HasAllFieldsNumeric()
               .HasKeyPrefix("app")
               .HasKeyPrefix("doc")
               .HasKey(x => x.Id)
               .HasFieldId(x => x.Name, 1)
               .HasFieldId(x => x.AttachmentIds, 2);
    }

    public static RedisEntityConfigurationBuilder<IDocument> ConfigureIDocument(this RedisEntityConfigurationBuilder<IDocument> builder)
    {
        return builder
               .HasKeyPrefix("app")
               .HasKeyPrefix("doc")
               .HasKey(x => x.Id, GuidBase64Utf8Formatter.Default)
               .HasFieldId(x => x.Name, 1)
               .HasFieldId(x => x.Character, 2);
    }
}