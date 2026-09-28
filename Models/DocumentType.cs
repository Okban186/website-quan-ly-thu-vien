namespace WebsiteQuanLyThuVien.Models;

public class DocumentType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}