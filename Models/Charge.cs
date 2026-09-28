using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class Charge
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }
    public Guid? LoanItemId { get; set; }

    public ChargeType ChargeType { get; set; }

    public decimal Amount { get; set; }

    public ChargeStatus Status { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LibraryMember Member { get; set; } = null!;
    public LoanItem? LoanItem { get; set; }

    public ICollection<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();
}