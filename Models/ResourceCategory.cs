namespace WebsiteQuanLyThuVien.Models;

public class ResourceCategory
{
    public Guid ResourceId { get; set; }
    public Guid CategoryId { get; set; }

    public Resource Resource { get; set; } = null!;
    public Category Category { get; set; } = null!;
}