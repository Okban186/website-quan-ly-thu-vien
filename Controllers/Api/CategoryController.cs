using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.Services;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Route("api/categories")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Route("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.SearchAsync(name, 6);

        return Ok(result);
    }
}