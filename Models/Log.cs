using System.ComponentModel.DataAnnotations;

namespace furkantural.Models;

public class Log
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The project/app name (e.g., "Main", "Crypt")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Project { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the log
    /// </summary>
    public DateTime Date { get; set; } = DateTime.Now;

    /// <summary>
    /// Log Level (Info, Warning, Error, Success)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Level { get; set; } = "Info";

    /// <summary>
    /// Short summary or title
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Full detail, stack trace, or JSON data
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// Client IP Address
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Request URI/Path
    /// </summary>
    [MaxLength(200)]
    public string? Path { get; set; }
}
