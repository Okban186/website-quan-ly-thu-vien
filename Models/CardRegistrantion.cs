using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class CardRegistration
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid? PaymentTransactionId { get; set; }

    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? IdDocumentNumber { get; set; }

    public Guid? IdDocumentFrontFileId { get; set; }
    public Guid? IdDocumentBackFileId { get; set; }
    public Guid? AvatarFileId { get; set; }

    public CardRegistrationStatus Status { get; set; }

    public string? RegistrationTokenHash { get; set; }
    public DateTime? RegistrationTokenExpiredAt { get; set; }
    public DateTime? RegistrationTokenRevokedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    public PaymentTransaction? PaymentTransaction { get; set; }

    public StorageFile? IdDocumentFrontFile { get; set; }
    public StorageFile? IdDocumentBackFile { get; set; }
    public StorageFile? AvatarFile { get; set; }

    public User? Reviewer { get; set; }
}