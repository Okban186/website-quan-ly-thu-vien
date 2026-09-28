namespace WebsiteQuanLyThuVien.Models;

public class ResourceAuthor
{
    public Guid ResourceId { get; set; }
    public Guid AuthorId { get; set; }

    public Resource Resource { get; set; } = null!;
    public Author Author { get; set; } = null!;
}