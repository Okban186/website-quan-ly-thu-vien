using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Services.Uploads;

namespace WebsiteQuanLyThuVien.Controllers.Api;


[Route("api/uploads")]

public class UploadController : Controller
{
    private readonly IUploadSessionService _uploadSessionService;

    public UploadController(IUploadSessionService uploadSessionService)
    {
        _uploadSessionService = uploadSessionService;
    }

    [HttpPost]
    public async Task<IActionResult> create()
    {
        return Ok(await _uploadSessionService.CreateAsync(UploadType.Avatar, null));
    }

    /// <summary>
    /// Hoàn tất phiên upload sau khi file đã được upload lên storage.
    /// </summary>
    /// <param name="id">ID của phiên upload.</param>
    /// <param name="cancellationToken">Token dùng để hủy thao tác.</param>
    /// <returns>Không có nội dung trả về.</returns>
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        await _uploadSessionService.CompleteAsync(id, cancellationToken);

        return NoContent();
    }
}