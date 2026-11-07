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

    [HttpGet("{id:guid}/copies")]
    public async Task<IActionResult> GetResourceCopies(
    Guid id,
    [FromQuery] int page = 1,
    CancellationToken cancellationToken = default)
    {
        var result = await _resourceService.GetResourceCopiesAsync(id, page);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id:guid}/suggestions")]
    public async Task<IActionResult> GetSuggestions(Guid id, CancellationToken cancellationToken)
    {


        var result = await _resourceService.GetSuggestionsAsync(id, cancellationToken);

        return Ok(result);
    }
}