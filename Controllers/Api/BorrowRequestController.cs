using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.Services;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Authorize]
[Route("api/borrow-requests")]
public class BorrowRequestContoller : Controller
{
    private readonly IReservationService _reservationService;
    public BorrowRequestContoller(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreateBorrowRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        return Ok(
            await _reservationService.CreateRequestAsync(Guid.Parse(userId), request)
        );
    }

    [HttpDelete("cancel-request-item/{id:guid}")]
    public async Task<IActionResult> CancelRequestItem(Guid id)
    {
        await _reservationService.CancelRequestItem(id);
        return Ok();

    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetMyRequests([FromQuery] string type, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _reservationService.GetBorrowItemList(userId, type, page, pageSize);
        return Ok(result);
    }

}