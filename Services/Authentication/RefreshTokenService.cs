using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Models.Authentication;

namespace WebsiteQuanLyThuVien.Services.Authentication;

public class RefreshTokenService : IRefreshTokenService
{

    private readonly JwtOptions _jwtOptions;
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(IOptions<JwtOptions> jwtOptions, ApplicationDbContext context)
    {
        _jwtOptions = jwtOptions.Value;
        _context = context;
    }
    public async Task<string> CreateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(64);

        var token = Convert.ToBase64String(tokenBytes);

        var tokenHash = HashToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,

            // Session mới
            FamilyId = Guid.NewGuid(),

            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),

            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        return token;
    }


    public async Task<RefreshTokenResult?> RotateAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(token);

        var currentToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (currentToken is null)
            return null;

        ///Token phát hiện bị hủy
        if (currentToken.RevokedAt.HasValue)
        {
            ///Hủy tất cả các token khác trong cùng session
            await RevokeFamilyAsync(currentToken.FamilyId, cancellationToken);

            return null;
        }

        ///Token hết hạn
        if (currentToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        ///Tạo token mới
        var newTokenBytes =
            RandomNumberGenerator.GetBytes(64);

        var newToken =
            Convert.ToBase64String(newTokenBytes);

        var newRefreshToken = new RefreshToken
        {
            UserId = currentToken.UserId,

            TokenHash =
                HashToken(newToken),

            // Giữ nguyên session
            FamilyId = currentToken.FamilyId,

            ExpiresAt =
                DateTime.UtcNow.AddDays(30),

            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newRefreshToken);

        ///revoke token cũ
        currentToken.RevokedAt =
            DateTime.UtcNow;


        currentToken.ReplacedByToken =
            newRefreshToken;

        await _context.SaveChangesAsync(
            cancellationToken);

        return new RefreshTokenResult
        {
            UserId = currentToken.UserId,
            RefreshToken = newToken
        };
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(token);

        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(
                    x => x.TokenHash == tokenHash,
                    cancellationToken);

        if (refreshToken is null)
            return;

        if (refreshToken.RevokedAt.HasValue)
            return;

        refreshToken.RevokedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private string HashToken(string token)
    {
        //Chuyển chuỗi token thô thành mảng bytes
        byte[] inputBytes = Encoding.UTF8.GetBytes(token);

        //Sử dụng thư viện mã hóa SHA-256 để băm dữ liệu
        byte[] hashBytes = SHA256.HashData(inputBytes);

        //Chuyển mảng bytes kết quả băm về dạng chuỗi Hexadecimal để lưu vào SQL Server
        return Convert.ToHexString(hashBytes);
    }


    private async Task RevokeFamilyAsync(Guid familyId, CancellationToken cancellationToken)
    {
        var tokens = await _context.RefreshTokens
                .Where(x =>
                    x.FamilyId == familyId &&
                    x.RevokedAt == null)
                .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        foreach (var token in tokens)
        {
            token.RevokedAt = now;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}