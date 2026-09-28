namespace WebsiteQuanLyThuVien.Models;

public class Staff
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string EmployeeCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}