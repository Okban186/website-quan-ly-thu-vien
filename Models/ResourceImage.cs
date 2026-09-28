namespace WebsiteQuanLyThuVien.Models;

public class ResourceImage
{
    public Guid Id { get; set; }

    public Guid ResourceId { get; set; }
    public Guid FileId { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }

    public Resource Resource { get; set; } = null!;
    public StorageFile File { get; set; } = null!;
}