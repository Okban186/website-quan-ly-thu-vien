using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.Services;
using WebsiteQuanLyThuVien.Services.Authentication;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Route("api/accounts")]
public class AccountController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IJwtTokenService _jwtTokenService;

    private readonly IAccountService _accountService;

    private readonly IUserRepository _userRepository;

    private readonly IRefreshTokenService _refreshTokenService;
    public AccountController(IAccountService accountService, IAuthenticationService authenticationService, IJwtTokenService jwtTokenService, IUserRepository userRepository, IRefreshTokenService refreshTokenService)
    {
        _authenticationService = authenticationService;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _accountService = accountService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    [ValidateAntiForgeryToken]
    ///Xóa tạm thời nếu muốn test bằng postman hoặc gọi api gì đó
    public async Task<IActionResult> Login(
    [FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Vui lòng nhập đầy đủ thông tin đăng nhập."
            });
        }

        var authenticationResult = await _authenticationService.AuthenticateAsync(request.Identifier, request.Password);

        // Access Token
        Response.Cookies.Append(
            "access_token",
            authenticationResult.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                MaxAge = request.RememberMe ? TimeSpan.FromMinutes(30) : null,
                IsEssential = true
            });

        // Refresh Token
        Response.Cookies.Append(
            "refresh_token",
            authenticationResult.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                MaxAge = request.RememberMe ? TimeSpan.FromDays(30) : null,
                IsEssential = true
            });

        return Ok(new
        {
            success = true
        });

    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await _jwtTokenService.RevokeCurrentTokenAsync(HttpContext);

        var refreshToken = Request.Cookies["refresh_token"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _refreshTokenService.RevokeAsync(refreshToken);
        }

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return Ok(new { success = true, message = "Logged out successfully" });
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        var result = await _refreshTokenService.RotateAsync(refreshToken);

        if (result is null)
        {
            Response.Cookies.Delete("refresh_token");
            Response.Cookies.Delete("access_token");

            return Unauthorized();
        }

        var user = await _userRepository.GetByIdAsync(result.UserId);

        if (user is null)
        {
            return Unauthorized();
        }

        var roles = await _userRepository.GetRolesAsync(user.Id);

        var accessToken = _jwtTokenService.GenerateToken(user, roles);

        Response.Cookies.Append(
            "access_token",
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(30),
                IsEssential = true
            });

        Response.Cookies.Append(
            "refresh_token",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(30),
                IsEssential = true
            });

        return Ok(new
        {
            success = true
        });
    }

    [HttpPost]
    [Authorize]
    [Route("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        await _accountService.ChangePasswordAsync(userId, request);

        return Ok();


    }

    [HttpPut]
    [Authorize]
    [Route("contact")]
    public async Task<IActionResult> UpdateContact([FromBody] UpdateUserContactRequest request)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _accountService.UpdateContactAsync(userId, request);
        return Ok();

    }

    [HttpPost]
    [AuthorizeExceptRole("ADMIN")]
    [Route("report-lost")]
    public async Task<IActionResult> ReportLostCard()
    {

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _accountService.ReportLostCardAsync(userId);
        return Ok();

    }

}

