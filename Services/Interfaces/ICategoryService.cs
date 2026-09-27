using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Services;

public interface ICategoryService
{
    Task<PagedResult<CategoryListItem>> GetPagedAsync(string? keyword, string? sortBy, int page, int pageSize, CancellationToken cancellationToken = default);
    
    Task<List<LookupResponse>> SearchAsync(string? name,int limit,CancellationToken cancellationToken = default);
}