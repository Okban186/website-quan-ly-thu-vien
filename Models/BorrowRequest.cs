using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class BorrowRequest
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public BorrowRequestType RequestType { get; set; }
    public BorrowRequestStatus Status { get; set; }

    public Guid? ProcessedBy { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LibraryMember Member { get; set; } = null!;
    public User? Processor { get; set; }

    public ICollection<BorrowRequestItem> Items { get; set; } = new List<BorrowRequestItem>();

    public Delivery? Delivery { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}