using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.Services;

namespace WebsiteQuanLyThuVien.Controllers.Mvc;

public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    public async Task<IActionResult> Index(string? keyword, string? sortBy, int page = 1, int pageSize = 10)
    {
        PagedResult<CategoryListItem> pagedResult = await _categoryService.GetPagedAsync(keyword, sortBy, page, pageSize);
        return View(pagedResult);
    }

}