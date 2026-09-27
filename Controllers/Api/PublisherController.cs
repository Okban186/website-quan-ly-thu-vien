using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Services;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Route("api/publishers/")]
public class PublisherController : Controller
{


    private readonly IPublisherService _publisherService;

    public PublisherController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] string? name)
    {
        var result = await _publisherService.SearchAsync(name, 6);
        return Ok(result);
    }
}