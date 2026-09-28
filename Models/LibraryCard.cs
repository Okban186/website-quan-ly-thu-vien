using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class LibraryCard
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public string CardNumber { get; set; } = null!;

    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }

    public LibraryCardStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LibraryMember Member { get; set; } = null!;
}