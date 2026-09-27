using Microsoft.AspNetCore.Mvc.RazorPages;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Repositories;

namespace WebsiteQuanLyThuVien.Services;

public class CategoryService : ICategoryService
{

    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<PagedResult<CategoryListItem>> GetPagedAsync(string? keyword, string? sortBy, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var allowedPageSizes = new[] { 10, 12, 16 };

        if (!allowedPageSizes.Contains(pageSize))
        {
            pageSize = 10;
        }
        return await _categoryRepository.GetPagedAsync(keyword, sortBy, page, pageSize, cancellationToken);
    }

    public async Task<List<LookupResponse>> SearchAsync(string? name, int limit, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.SearchAsync(name, limit);
    }
}