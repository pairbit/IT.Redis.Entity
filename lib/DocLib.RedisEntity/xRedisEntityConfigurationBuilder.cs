using IT.Redis.Entity.Configurations;

namespace DocLib.RedisEntity;

public static class xRedisEntityConfigurationBuilder
{
    public static void ConfigureDocumentAnnotation(this RedisEntityConfigurationBuilder<DocumentAnnotation> builder)
    {
        builder.HasKeyPrefix("app")
               .HasKeyPrefix("doc")
               .HasKey(x => x.Id)
               .HasFieldId(x => x.Name, 1)
               .HasFieldId(x => x.AttachmentIds, 2)
               .Ignore(x => x.RedisKey)
               .Ignore(x => x.RedisKeyBits);
    }
}