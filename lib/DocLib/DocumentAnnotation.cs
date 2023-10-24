using IT.Collections.Equatable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocLib;

[Table("doc", Schema = "app")]
public record DocumentAnnotation
{
    private Guid _id;
    private byte[]? _redisKey;

    [NotMapped]
    public byte[]? RedisKey => _redisKey;

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
                _redisKey = null;
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