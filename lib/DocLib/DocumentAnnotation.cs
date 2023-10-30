using IT.Collections.Equatable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocLib;

[Table("doc", Schema = "app")]
public record DocumentAnnotation
{
    private Guid _id;
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Field 'DocumentAnnotation._redisKey' is never assigned to, and will always have its default value null
    private byte[]? _redisKey;
#pragma warning restore CS0649 // Field 'DocumentAnnotation._redisKey' is never assigned to, and will always have its default value null
#pragma warning restore IDE0044 // Add readonly modifier

    private byte _redisKeyBits;

    public byte[]? RedisKey => _redisKey;

    public byte RedisKeyBits => _redisKeyBits;

    [Key]
    [Column(Order = 0)]
    public Guid Id
    {
        get => _id;
        set 
        { 
            if (_id != value)
            {
                _id = value;
                _redisKeyBits = 1;
            }
        }
    }

    [Column(Order = 1)]
    public string Name { get; set; } = null!;

    [Column(Order = 2)]
    public EquatableList<int>? AttachmentIds { get; set; }
}

//app:attach:[Guid]:[Index]
[Table("attach", Schema = "app")]
public record DocumentAttachmentAnnotation
{
    [Key]
    [Column(Order = 0)]
    public Guid DocumentId { get; set; }

    [Key]
    [Column(Order = 1)]
    public int Index { get; set; }

    [Column(Order = 2)]
    public string Name { get; set; } = null!;

    [Column(Order = 3)]
    public byte[] Bytes { get; set; } = null!;
}