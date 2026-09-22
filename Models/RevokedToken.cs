using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanLyThuVien.Models;

[Table("revoked_tokens")]
public class RevokedToken
{

    [Key]
    [Column("jti")]
    public Guid Jti { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("revoked_at")]
    public DateTime RevokedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}