using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class DigitalResource
{
    public Guid Id { get; set; }

    public Guid ResourceId { get; set; }
    public Guid FileId { get; set; }

    public DigitalResourceType ResourceType { get; set; }
    public DigitalResourceAccessLevel AccessLevel { get; set; }
    public DigitalResourceStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Resource Resource { get; set; } = null!;
    public StorageFile File { get; set; } = null!;
}