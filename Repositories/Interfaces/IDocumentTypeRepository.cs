using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Repositories;

public interface IDocumenTypeRepository
{
    Task<List<LookupResponse>> SearchAsync(string? name, int limit = 6, CancellationToken cancellationToken = default);
}