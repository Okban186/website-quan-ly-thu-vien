using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public class CategoryRepository : ICategoryRepository
{

    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CategoryListItem>> GetPagedAsync(string? keyword, string? sortBy, int page, int pageSize, CancellationToken cancellationToken = default)
    {

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = _context.Categories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(x => x.Name.Contains(keyword));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        switch (sortBy)
        {
            case "name-asc":
                query = query.OrderBy(x => x.Name);
                break;

            case "name-desc":
                query = query.OrderByDescending(x => x.Name);
                break;

            case "bookCount-asc":
                query = query.OrderBy(x => x.BookCategories.Count());
                break;

            case "bookCount-desc":
                query = query.OrderByDescending(x => x.BookCategories.Count());
                break;

            default:
                query = query
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Name);
                break;
        }

        var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new CategoryListItem
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            DisplayOrder = x.DisplayOrder,

            BookCount = x.BookCategories.Count()
        })
        .ToListAsync(cancellationToken);

        return new PagedResult<CategoryListItem>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
    
   public async Task<List<LookupResponse>> SearchAsync(string? name,int limit = 5,CancellationToken cancellationToken = default)
    {

        var query = _context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var keyword = name.Trim();

            query = query.Where(category => category.Name.Contains(keyword));
        }

        return await query
            .OrderBy(category => category.Name)
            .Take(limit)
            .Select(category => new LookupResponse
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToListAsync(cancellationToken);
    }
}