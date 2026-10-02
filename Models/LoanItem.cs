using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class LoanItem
{
    public Guid Id { get; set; }

    public Guid LoanId { get; set; }
    public Guid LibraryItemId { get; set; }

    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }

    public LoanItemStatus Status { get; set; }

    public ItemCondition? ConditionAtLoan { get; set; }
    public string? ConditionNoteAtLoan { get; set; }
    public ItemCondition? ConditionAtReturn { get; set; }
    public string? ConditionNoteAtReturn { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Loan Loan { get; set; } = null!;
    public LibraryItem LibraryItem { get; set; } = null!;

    public ICollection<LoanRenewal> Renewals { get; set; } = new List<LoanRenewal>();

    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
}