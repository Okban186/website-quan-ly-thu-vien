using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Repositories.Filters;
using WebsiteQuanLyThuVien.Services.Storage;

namespace WebsiteQuanLyThuVien.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ResourceFilterExpressionService _filterExpressionService;

    private readonly IFileStorageService _fileStorageService;

    public ResourceRepository(ApplicationDbContext context, ResourceFilterExpressionService filterExpressionService, IFileStorageService fileStorageService)
    {
        _context = context;
        _filterExpressionService = filterExpressionService;
        _fileStorageService = fileStorageService;
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

        bool hasResourceAvailable = await _context.LibraryItems.AnyAsync(li => li.Status == LibraryItemStatus.AVAILABLE);

        var totalCount =
            await query.CountAsync(cancellationToken);


        // PAGINATION


        var page = request.Page < 1
            ? 1
            : request.Page;

        var pageSize = request.PageSize < 1
            ? 12
            : request.PageSize;

        var pagedData = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(resource => new
            {
                resource.Id,
                resource.Title,
                resource.PublicationYear,
                DocumentTypeName = resource.DocumentType != null ? resource.DocumentType.Name : null,
                PublisherName = resource.Publisher != null ? resource.Publisher.Name : null,
                Authors = resource.ResourceAuthors.Select(ba => ba.Author.Name).ToList(),
                Categories = resource.ResourceCategories.Select(bc => bc.Category.Name).ToList(),

                // Lấy ObjectKey thô của ảnh chính
                ObjectKey = resource.ResourceImages
                    .OrderByDescending(img => img.IsPrimary)
                    .ThenBy(img => img.DisplayOrder)
                    .Select(img => img.File != null ? img.File.ObjectKey : null)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var items = await Task.WhenAll(pagedData.Select(async resource => new ResourceSearchItemDto
        {
            Id = resource.Id,
            Title = resource.Title,
            PublicationYear = resource.PublicationYear,
            DocumentTypeName = resource.DocumentTypeName,
            PublisherName = resource.PublisherName,
            Authors = resource.Authors,
            Categories = resource.Categories,
            HasResourceAvaiable = hasResourceAvailable,
            CoverUrl = !string.IsNullOrEmpty(resource.ObjectKey)
                ? await _fileStorageService.CreatePublicGetUrlAsync(resource.ObjectKey)
                : null
        }));

        return new PagedResult<ResourceSearchItemDto>
        {
            Items = items.ToList(),
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}