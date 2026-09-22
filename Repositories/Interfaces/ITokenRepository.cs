namespace WebsiteQuanLyThuVien.Repositories;

public interface ITokenRepository
{
    Task RevokeAsync(Guid jti, Guid userId, DateTime expiresAt, CancellationToken cancellationToken = default);

    Task<bool> IsRevokedAsync(Guid jti, CancellationToken cancellationToken = default);
}