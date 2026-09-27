using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Services;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Route("api/document-types")]
public class DocumentTypeController : Controller
{
    private readonly IDocumentTypeService _documentTypeService;

    public DocumentTypeController(IDocumentTypeService documentTypeService)
    {
        _documentTypeService = documentTypeService;
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] string? name)
    {
        var result = await _documentTypeService.SearchAsync(name, 6);
        return Ok(result);
    }

}