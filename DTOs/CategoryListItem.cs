namespace WebsiteQuanLyThuVien.DTOs;

public class CategoryListItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public int ResourceCount { get; set; }
}