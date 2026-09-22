using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Common;

namespace WebsiteQuanLyThuVien.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.Username == identifier ||
                u.Email == identifier);
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }

    public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return;

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            throw new BusinessException(Account.UsernameAlreadyExists.Message, Account.UsernameAlreadyExists.StatusCode);

        return user;
    }
}