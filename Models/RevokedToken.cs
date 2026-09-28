namespace WebsiteQuanLyThuVien.Models;

public class RevokedToken
{
    public Guid Id { get; set; }

    public Guid? Jti { get; set; } = null!;
    public Guid UserId { get; set; }

    public DateTime ExpiresAt { get; set; }
    public DateTime RevokedAt { get; set; }

    public User User { get; set; } = null!;
}