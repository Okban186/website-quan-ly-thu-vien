using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Services;


namespace WebsiteQuanLyThuVien.Controllers.Api;


[Route("api/charges")]
[AuthorizeExceptRole("ADMIN")]
public class ChargesController : Controller
{
    private readonly IChargeService _chargeService;

    public ChargesController(IChargeService chargeService)
    {
        _chargeService = chargeService;
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyCharges([FromQuery] ChargeStatus status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(typeof(ChargeStatus), status))
        {
            throw new BusinessException("Trạng thái khoản phí không hợp lệ");
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            throw new BusinessException("", StatusCodes.Status401Unauthorized);
        }

        var result = await _chargeService.GetMemberChargesAsync(userId, status, page, pageSize);

        return Ok(result);
    }
}