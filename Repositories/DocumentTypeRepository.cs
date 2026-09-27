using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Repositories;

public class DocumentTypeRepository : IDocumenTypeRepository
{

    private readonly ApplicationDbContext _context;
       public DocumentTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<LookupResponse>> SearchAsync(string? name, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.DocumentTypes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var keyword = name.Trim();

            query = query.Where(docType => docType.Name.Contains(keyword));
        }

        return await query
            .OrderBy(docType => docType.Name)
            .Take(limit)
            .Select(docType => new LookupResponse
            {
                Id = docType.Id,
                Name = docType.Name
            })
            .ToListAsync(cancellationToken);
    }
}