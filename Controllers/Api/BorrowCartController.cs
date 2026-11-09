using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Services;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Route("api/borrow-carts")]
[Authorize]
public class BorrowCartController : Controller
{
    private readonly IBorrowCartService _borrowCartService;
    public BorrowCartController(IBorrowCartService borrowCartService)
    {
        _borrowCartService = borrowCartService;
    }
    [HttpGet("queue")]
    public async Task<IActionResult> GetMyQueueAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string? search = null)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        var result = await _borrowCartService.GetCartAsync(Guid.Parse(userIdString), search, page, pageSize);
        return Ok(result);
    }


    [HttpPost("queue/bulk-delete")]
    public async Task<IActionResult> DeleteBorrowRequestItem([FromBody] List<Guid> resourceIds)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        await _borrowCartService.RemoveFromCartAsync(Guid.Parse(userIdString), resourceIds);
        return Ok();

    }

    [HttpPost("queue/{resourceId:guid}")]
    public async Task<IActionResult> AddtoCart(Guid resourceId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        await _borrowCartService.AddToCartAsync(Guid.Parse(userIdString), resourceId);
        return Ok();
    }

}