using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class LibraryItemStatusHistory
{
    public Guid Id { get; set; }

    public Guid LibraryItemId { get; set; }

    public LibraryItemStatus? OldStatus { get; set; }
    public LibraryItemStatus NewStatus { get; set; }

    public Guid? ChangedBy { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public LibraryItem LibraryItem { get; set; } = null!;
    public User? ChangedByUser { get; set; }
}