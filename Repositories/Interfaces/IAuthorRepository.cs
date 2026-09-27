using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Repositories;

public interface IAuthorRepository
{
    Task<List<LookupResponse>> SearchAsync(string? name, int limit = 6, CancellationToken cancellationToken = default);
}