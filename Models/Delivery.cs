using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class Delivery
{
    public Guid Id { get; set; }

    public Guid BorrowRequestId { get; set; }

    public string Address { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string RecipientPhone { get; set; } = null!;

    public DeliveryStatus Status { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public BorrowRequest BorrowRequest { get; set; } = null!;
}