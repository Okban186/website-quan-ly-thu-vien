namespace WebsiteQuanLyThuVien.ViewModels;

public class ResourceViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? CoverUrl { get; set; }

    public string? AuthorName { get; set; }

    public string? PublisherName { get; set; }

    public int? PublicationYear { get; set; }
}

