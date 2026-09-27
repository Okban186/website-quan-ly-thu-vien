
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public interface ICategoryRepository
{
    Task<PagedResult<CategoryListItem>> GetPagedAsync(string? keyword, string? sortBy, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<List<LookupResponse>> SearchAsync(string? name, int limit = 6, CancellationToken cancellationToken = default);
}