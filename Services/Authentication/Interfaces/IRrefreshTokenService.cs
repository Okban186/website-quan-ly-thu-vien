using WebsiteQuanLyThuVien.DTOs;

namespace WebsiteQuanLyThuVien.Services.Authentication;

public interface IRefreshTokenService
{
    /// <summary>
    /// Tạo refesh token mới
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> CreateAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo refesh token rotate
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RefreshTokenResult?> RotateAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Khi logout revoke lại
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}