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
}