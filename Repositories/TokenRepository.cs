using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQuanLyThuVien.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly ApplicationDbContext _context;

    public TokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RevokeAsync(Guid jti, Guid userId, DateTime expiresAt, CancellationToken cancellationToken = default)
    {


        var token = new RevokedToken
        {
            Jti = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            RevokedAt = DateTime.UtcNow
        };

        _context.RevokedTokens.Add(token);



        var affectedRows = await _context.SaveChangesAsync();


    }

    public Task<bool> IsRevokedAsync(Guid jti, CancellationToken cancellationToken = default)
    {
        return _context.RevokedTokens.AnyAsync(x => x.Jti == jti);
    }


}