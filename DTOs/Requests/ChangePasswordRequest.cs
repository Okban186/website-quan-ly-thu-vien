using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyThuVien.DTOs.Requests;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; }

    public string NewPassword { get; set; }

    public string ConfirmPassword { get; set; }
}
