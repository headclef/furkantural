using System.ComponentModel.DataAnnotations;
namespace furkantural.Domain.Entities.Common;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
    [Required]
    public bool IsDeleted { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(3);
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}