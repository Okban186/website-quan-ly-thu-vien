using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Models.Authentication;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.Services.Authentication;

namespace WebsiteQuanLyThuVien.Services.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ITokenRepository _tokenRepository;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions, ITokenRepository tokenRepository)
    {
        _jwtOptions = jwtOptions.Value;
        _tokenRepository = tokenRepository;
    }

    public string GenerateToken(User user, IReadOnlyList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("full_name", user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            _jwtOptions.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    public async Task RevokeCurrentTokenAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
    {


        var jtiValue = httpContext.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        var userIdValue = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



        if (!Guid.TryParse(jtiValue, out var jti))


            return;


        if (!Guid.TryParse(userIdValue, out var userId))


            return;


        var token = httpContext.Request.Cookies["access_token"];



        if (string.IsNullOrEmpty(token))


            return;


        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);



        await _tokenRepository.RevokeAsync(jti, userId, jwt.ValidTo.ToUniversalTime());


    }


}