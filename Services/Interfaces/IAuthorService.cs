using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Services;

public interface IAuthorService
{
    Task<List<LookupResponse>> SearchAsync(string? name,int limit,CancellationToken cancellationToken = default);
}