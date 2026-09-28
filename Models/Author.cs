namespace WebsiteQuanLyThuVien.Models;

public class Author
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Biography { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ResourceAuthor> ResourceAuthors { get; set; } = new List<ResourceAuthor>();
}