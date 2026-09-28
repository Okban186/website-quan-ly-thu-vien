using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Repositories.Filters;

namespace WebsiteQuanLyThuVien.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ResourceFilterExpressionService _filterExpressionService;

    public ResourceRepository(ApplicationDbContext context, ResourceFilterExpressionService filterExpressionService)
    {
        _context = context;
        _filterExpressionService = filterExpressionService;
    }

    /// <summary>
    /// Tìm kiếm sách nâng cao dựa trên:
    /// - Keyword
    /// - FilterExpression
    /// - Năm xuất bản
    /// - Trạng thái khả dụng
    /// - Sắp xếp
    /// - Phân trang
    /// </summary>
    public async Task<PagedResult<ResourceSearchItemDto>> AdvancedSearchAsync(ReaderResourceSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Resources.AsNoTracking().AsQueryable();


        // KEYWORD


        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(Resource => Resource.Title.Contains(keyword));
        }


        // ADVANCED FILTER


        if (!string.IsNullOrWhiteSpace(request.FilterExpression))
        {
            var predicate = await _filterExpressionService.BuildAsync(request.FilterExpression, cancellationToken);

            query = query.Where(predicate);
        }


        // PUBLICATION YEAR


        if (request.PublicationYear.HasValue)
        {
            query = query.Where(Resource => Resource.PublicationYear == request.PublicationYear.Value);
        }


        // AVAILABILITY STATUS


        switch (request.Status?.ToLowerInvariant())
        {
            case "available":

                query = query.Where(
                    Resource => Resource.LibraryItems.Any(
                        copy => copy.Status.ToString() == "AVAILABLE"));

                break;

            case "unavailable":

                query = query.Where(
                    Resource => !Resource.LibraryItems.Any(
                        copy => copy.Status.ToString() == "AVAILABLE"));

                break;
        }


        // SORTING


        query = request.Sort?.ToLowerInvariant() switch
        {
            "title" =>
                query.OrderBy(Resource => Resource.Title),

            "year" =>
                query
                    .OrderByDescending(Resource => Resource.PublicationYear)
                    .ThenBy(Resource => Resource.Title),

            "newest" =>
                query.OrderByDescending(Resource => Resource.CreatedAt),

            _ =>
                query.OrderBy(Resource => Resource.Title)
        };


        // TOTAL COUNT


        var totalCount =
            await query.CountAsync(cancellationToken);


        // PAGINATION


        var page = request.Page < 1
            ? 1
            : request.Page;

        var pageSize = request.PageSize < 1
            ? 12
            : request.PageSize;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(Resource => new ResourceSearchItemDto
            {
                Id = Resource.Id,

                Title = Resource.Title,

                PublicationYear = Resource.PublicationYear,

                DocumentTypeName = Resource.DocumentType.Name,

                PublisherName = Resource.Publisher != null
                    ? Resource.Publisher.Name
                    : null,

                Authors = Resource.ResourceAuthors
                    .Select(ba => ba.Author.Name)
                    .ToList(),

                Categories = Resource.ResourceCategories
                    .Select(bc => bc.Category.Name)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ResourceSearchItemDto>
        {
            Items = items,

            TotalItems = totalCount,

            Page = page,

            PageSize = pageSize
        };
    }
}