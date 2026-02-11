using System.ComponentModel.DataAnnotations;
using furkantural.Domain.Entities.Common;
namespace furkantural.Domain.Entities;

public class Log : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Project { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    [Required]
    [MaxLength(20)]
    public string Level { get; set; } = "Info";
    [Required]
    [MaxLength(200)]
    public string Message { get; set; } = string.Empty;
    public string? Detail { get; set; }
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    [MaxLength(200)]
    public string? Path { get; set; }
}