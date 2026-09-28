using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class Loan
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }
    public Guid? BorrowRequestId { get; set; }
    public Guid CreatedBy { get; set; }

    public LoanStatus Status { get; set; }

    public DateTime BorrowedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LibraryMember Member { get; set; } = null!;
    public BorrowRequest? BorrowRequest { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
}