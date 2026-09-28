namespace WebsiteQuanLyThuVien.Models;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ResourceCategory> ResourceCategories { get; set; } = new List<ResourceCategory>();
}