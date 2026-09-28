using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class PaymentTransaction
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public string TransactionCode { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; }
    public PaymentTransactionStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LibraryMember Member { get; set; } = null!;

    public ICollection<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();

    public CardRegistration? CardRegistration { get; set; }
}