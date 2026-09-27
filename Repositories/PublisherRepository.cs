using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly ApplicationDbContext _context;

    public PublisherRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<LookupResponse>> SearchAsync(string? name, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.Publishers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var keyword = name.Trim();

            query = query.Where(publisher => publisher.Name.Contains(keyword));
        }

        return await query
            .OrderBy(publisher => publisher.Name)
            .Take(limit)
            .Select(publisher => new LookupResponse
            {
                Id = publisher.Id,
                Name = publisher.Name
            })
            .ToListAsync(cancellationToken);
    }

}