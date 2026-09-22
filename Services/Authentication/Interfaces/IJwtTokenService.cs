namespace WebsiteQuanLyThuVien.Services.Authentication;

using WebsiteQuanLyThuVien.Models;

public interface IJwtTokenService
{
    string GenerateToken(User user, IReadOnlyList<string> roles);

    /// <summary>
    /// Hàm lấy jwt token hiện tại để thu hồi
    /// </summary>
    /// <param name="httpContext">nơi lấy jwt</param>
    /// <returns></returns>
    Task RevokeCurrentTokenAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
}