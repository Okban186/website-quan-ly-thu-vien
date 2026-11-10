namespace WebsiteQuanLyThuVien.Controllers.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Services;
using WebsiteQuanLyThuVien.ViewModels;

[Authorize]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    public async Task<IActionResult> Index()
    {
        string? userStringId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        if (userStringId == null) throw new BusinessException("Tài khoản không hợp lệ", StatusCodes.Status403Forbidden);

        Guid userId = Guid.Parse(userStringId);
        UserAccountViewModel viewModel = await _accountService.GetUserProfileAsync(userId);
        return View(viewModel);
    }
}