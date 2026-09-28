namespace WebsiteQuanLyThuVien.DTOs;

public class ResourceSearchItemDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? CoverUrl { get; set; }

    public string? DocumentTypeName { get; set; }

    public IReadOnlyList<string> Authors { get; set; } = [];

    public string? PublisherName { get; set; }

    public int? PublicationYear { get; set; }

    public IReadOnlyList<string> Categories { get; set; } = [];

    public int AvailableCopies { get; set; }

    public int TotalCopies { get; set; }
}