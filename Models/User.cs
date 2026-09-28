using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? CitizenId { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }

    public Guid? AvatarFileId { get; set; }

    public UserStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public StorageFile? AvatarFile { get; set; }

    public Staff? Staff { get; set; }
    public LibraryMember? LibraryMember { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<BorrowRequest> ProcessedBorrowRequests { get; set; } = new List<BorrowRequest>();

    public ICollection<Loan> CreatedLoans { get; set; } = new List<Loan>();

    public ICollection<LoanRenewal> LoanRenewals { get; set; } = new List<LoanRenewal>();

    public ICollection<CardRegistration> CardRegistrations { get; set; } = new List<CardRegistration>();

    public ICollection<CardRegistration> ReviewedCardRegistrations { get; set; } = new List<CardRegistration>();

    public ICollection<NotificationRecipient> NotificationRecipients { get; set; } = new List<NotificationRecipient>();

    public ICollection<CopyIssue> ReportedCopyIssues { get; set; } = new List<CopyIssue>();

    public ICollection<CopyIssue> ResolvedCopyIssues { get; set; } = new List<CopyIssue>();

    public ICollection<LibraryItemStatusHistory> LibraryItemStatusHistories { get; set; } = new List<LibraryItemStatusHistory>();

    public ICollection<UploadSession> UploadSessions { get; set; } = new List<UploadSession>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<RevokedToken> RevokedTokens { get; set; } = new List<RevokedToken>();
}