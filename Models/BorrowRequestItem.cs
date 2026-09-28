using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class BorrowRequestItem
{
    public Guid Id { get; set; }

    public Guid BorrowRequestId { get; set; }
    public Guid ResourceId { get; set; }
    public Guid? LibraryItemId { get; set; }

    public BorrowRequestItemStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public BorrowRequest BorrowRequest { get; set; } = null!;
    public Resource Resource { get; set; } = null!;
    public LibraryItem? LibraryItem { get; set; }
}