namespace WebsiteQuanLyThuVien.Models;

public class LoanRenewal
{
    public Guid Id { get; set; }

    public Guid LoanItemId { get; set; }

    public DateTime OldDueAt { get; set; }
    public DateTime NewDueAt { get; set; }

    public Guid RenewedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public LoanItem LoanItem { get; set; } = null!;
    public User RenewedByUser { get; set; } = null!;
}