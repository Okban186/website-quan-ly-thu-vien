using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanLyThuVien.Models;

[Table("storage_files")]
public class StorageFile
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("file_key")]
    public string FileKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("file_name")]
    public string FileName { get; set; } = string.Empty;

    [Column("file_url")]
    public string? FileUrl { get; set; }

    [MaxLength(100)]
    [Column("mime_type")]
    public string? MimeType { get; set; }

    [Column("file_size")]
    public long? FileSize { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}