namespace WebsiteQuanLyThuVien.Models;

public class PaymentItem
{
    public Guid Id { get; set; }

    public Guid PaymentId { get; set; }
    public Guid ChargeId { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public PaymentTransaction Payment { get; set; } = null!;
    public Charge Charge { get; set; } = null!;
}