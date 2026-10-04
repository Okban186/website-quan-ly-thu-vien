using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Route("api/resources")]
public class ResourceController : Controller
{

    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpPost]
    [Route("advance-search")]
    [AllowAnonymous]
    public async Task<IActionResult> ResourcesSearchAsync([FromBody] ReaderResourceSearchRequest request)
    {
        var result = await _resourceService.AdvancedSearchAsync(request);

        return Ok(result);
    }
}