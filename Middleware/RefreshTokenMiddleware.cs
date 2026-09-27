
using System.IdentityModel.Tokens.Jwt;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.Services.Authentication;

namespace WebsiteQuanLyThuVien.Middleware;
/// <summary>
/// 
/// </summary>
public class RefreshTokenMiddleware
{
    private readonly RequestDelegate _next;

    public RefreshTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRefreshTokenService refreshTokenService, IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        var accessToken = context.Request.Cookies["access_token"];
        var refreshToken = context.Request.Cookies["refresh_token"];

        // Không có refresh token → không thể refresh.
        if (string.IsNullOrEmpty(refreshToken))
        {
            await _next(context);
            return;
        }

        // Access token chưa hết hạn → tiếp tục request bình thường.
        if (!IsExpired(accessToken))
        {
            await _next(context);
            return;
        }

        // Access token hết hạn → rotation refresh token.
        var result = await refreshTokenService.RotateAsync(refreshToken);

        if (result is null)
        {
            context.Response.Cookies.Delete("access_token");
            context.Response.Cookies.Delete("refresh_token");

            await _next(context);
            return;
        }

        var user = await userRepository.GetByIdAsync(result.UserId);

        if (user is null)
        {
            context.Response.Cookies.Delete("access_token");
            context.Response.Cookies.Delete("refresh_token");

            await _next(context);
            return;
        }

        var roles = await userRepository.GetRolesAsync(user.Id);

        var newAccessToken = jwtTokenService.GenerateToken(user, roles);

        context.Items["refreshed_access_token"] = newAccessToken;
        context.Response.Cookies.Append(
            "access_token",
            newAccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(30),
                IsEssential = true
            });

        context.Response.Cookies.Append(
            "refresh_token",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(30),
                IsEssential = true
            });

        await _next(context);
    }

    private static bool IsExpired(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return true;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            return token.ValidTo <= DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}
