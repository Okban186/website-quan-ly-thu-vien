using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class CardRegistration
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string IdDocumentNumber { get; set; } = null!;

    public Guid? IdDocumentFrontFileId { get; set; }

    public Guid? IdDocumentBackFileId { get; set; }

    public Guid? AvatarFileId { get; set; }

    public CardRegistrationStatus Status { get; set; } = CardRegistrationStatus.PENDING;

    public DateTime SubmittedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public Guid? ReviewedBy { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }


    public User? Reviewer { get; set; }

    public StorageFile? AvatarFile { get; set; }

    public StorageFile? IdDocumentFrontFile { get; set; }

    public StorageFile? IdDocumentBackFile { get; set; }
}