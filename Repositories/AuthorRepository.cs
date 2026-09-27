using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;

    public AuthorRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<LookupResponse>> SearchAsync(string? name, int limit, CancellationToken cancellationToken = default)
    {


        var query = _context.Authors.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var keyword = name.Trim();

            query = query.Where(author => author.Name.Contains(keyword));
        }

        var result = await query
        .OrderBy(author => author.Name)
        .Take(limit)
        .Select(author => new LookupResponse
        {
            Id = author.Id,
            Name = author.Name
        })
        .ToListAsync(cancellationToken);



        return result;
    }
}