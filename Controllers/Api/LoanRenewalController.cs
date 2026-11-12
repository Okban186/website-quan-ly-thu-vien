using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Services;

namespace WebsiteQuanLyThuVien.Controllers;

[Route("api/loan-renewals")]
[AuthorizeExceptRole("ADMIN")]
[Authorize]
public class LoanRenewalController : Controller
{

    private readonly ILoanRenewalService _loanRenewalService;

    public LoanRenewalController(ILoanRenewalService loanRenewalService)
    {
        _loanRenewalService = loanRenewalService;
    }

    [HttpPost("renew/{loanItemId:guid}")]
    public async Task<IActionResult> Renew([FromRoute] Guid loanItemId)
    {


        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim);

        bool result = await _loanRenewalService.RenewLoanItemAsync(loanItemId, userId);

        return Ok();


    }

}