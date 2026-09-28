using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class LibraryItem
{
    public Guid Id { get; set; }

    public Guid ResourceId { get; set; }
    public Guid? LocationId { get; set; }

    public string Barcode { get; set; } = null!;

    public LibraryItemStatus Status { get; set; }
    public ItemCondition ItemCondition { get; set; }

    public DateTime? AcquiredAt { get; set; }
    public decimal? AcquisitionPrice { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Resource Resource { get; set; } = null!;
    public Location? Location { get; set; }

    public ICollection<BorrowRequestItem> BorrowRequestItems { get; set; } = new List<BorrowRequestItem>();

    public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();

    public ICollection<CopyIssue> CopyIssues { get; set; } = new List<CopyIssue>();

    public ICollection<LibraryItemStatusHistory> StatusHistories { get; set; } = new List<LibraryItemStatusHistory>();
}