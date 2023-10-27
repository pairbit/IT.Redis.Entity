using IT.Collections.Equatable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocLib;

[Table("doc", Schema = "app")]
public record DocumentAnnotation
{
    private Guid _id;
    private byte[]? _redisKey;
#pragma warning disable IDE0052 // Remove unread private members
    private byte _redisKeyBits;
#pragma warning restore IDE0052 // Remove unread private members

    [NotMapped]
    public byte[]? RedisKey => _redisKey;

    [NotMapped]
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