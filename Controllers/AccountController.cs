using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.Services.Authentication;


namespace WebsiteQuanLyThuVien.Controllers;

public class AccountController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IJwtTokenService _jwtTokenService;

    private readonly IUserRepository _userRepository;

    private readonly IRefreshTokenService _refreshTokenService;
    public AccountController(IAuthenticationService authenticationService, IJwtTokenService jwtTokenService, IUserRepository userRepository, IRefreshTokenService refreshTokenService)
    {
        _authenticationService = authenticationService;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost]
    [AllowAnonymous]
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

        try
        {
            var authenticationResult = await _authenticationService.AuthenticateAsync(request.Identifier, request.Password);

            if (authenticationResult is null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Tên đăng nhập/email hoặc mật khẩu không chính xác."
                });
            }

            // Access Token
            Response.Cookies.Append(
                "access_token",
                authenticationResult.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    MaxAge = TimeSpan.FromMinutes(30),
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
                    MaxAge = TimeSpan.FromDays(30),
                    IsEssential = true
                });

            return Ok(new
            {
                success = true
            });
        }
        catch (BusinessException ex)
        {
            return StatusCode(ex.StatusCode, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    [Authorize]

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

        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
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


}

