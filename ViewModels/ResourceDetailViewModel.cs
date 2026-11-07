using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.DTOs.ViewModels;

public class ResourceDetailViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ISBN { get; set; } = string.Empty;

    public string PhysicalDescription { get; set; } = String.Empty;

    public List<ImageResourceItem> Images { get; set; } = new List<ImageResourceItem>();

    public string? DocumentTypeName { get; set; }

    public IReadOnlyList<string> Authors { get; set; } = [];

    public string? Description { get; set; }

    public string? PublisherName { get; set; }

    public int? PublicationYear { get; set; }

    public IReadOnlyList<string> Categories { get; set; } = [];

    public int AvailableCopies { get; set; }

    public int TotalCopies { get; set; }

    public record ImageResourceItem(string url, int display_order, bool isPrimary);

    public record ResourceCopyItem(string barcode, LibraryItemStatus status, string locationName);
}

