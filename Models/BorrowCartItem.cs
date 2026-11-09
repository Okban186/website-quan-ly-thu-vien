namespace WebsiteQuanLyThuVien.Models;

public class BorrowCartItem
{
    public Guid MemberId { get; set; }
    public Guid ResourceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual LibraryMember Member { get; set; } = null!;
    public virtual Resource Resource { get; set; } = null!;
}
