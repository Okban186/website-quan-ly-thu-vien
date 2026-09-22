namespace WebsiteQuanLyThuVien.Services.Authentication;

public class AuthenticationResult
{
    public string AccessToken { get; init; } = String.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}