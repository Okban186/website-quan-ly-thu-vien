namespace WebsiteQuanLyThuVien.Models;

public class Publisher
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}