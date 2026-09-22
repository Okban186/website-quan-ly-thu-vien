namespace WebsiteQuanLyThuVien.Repositories;


using WebsiteQuanLyThuVien.Models;


public interface IUserRepository
{
    Task<User?> GetByUsernameOrEmailAsync(string identifier, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}