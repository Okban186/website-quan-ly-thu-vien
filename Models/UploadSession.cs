using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public sealed class UploadSession
{
    public Guid Id { get; set; }

    public Guid? CardRegistrationId { get; set; }

    public Guid? UserId { get; set; }

    public string ObjectKey { get; set; } = null!;

    public UploadType UploadType { get; set; }

    public UploadSessionStatus Status { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }


    // Navigation properties

    public User? User { get; set; }

    public CardRegistration? CardRegistration { get; set; }
}