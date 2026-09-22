using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyThuVien.DTOs.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập hoặc email.")]
    public string Identifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string Password { get; set; } = string.Empty;
}

