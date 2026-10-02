using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class CopyIssue
{
    public Guid Id { get; set; }

    public Guid LibraryItemId { get; set; }
    public Guid LoanItemId { get; set; }
    public Guid ReportedBy { get; set; }

    public CopyIssueType IssueType { get; set; }

    public string? Description { get; set; }

    public CopyIssueStatus Status { get; set; }

    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LibraryItem LibraryItem { get; set; } = null!;

    public User Reporter { get; set; } = null!;
    public User? Resolver { get; set; }
}