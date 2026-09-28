using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Repositories.Filters;

namespace WebsiteQuanLyThuVien.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly BookFilterExpressionService _filterExpressionService;

    public BookRepository(ApplicationDbContext context, BookFilterExpressionService filterExpressionService)
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
    public async Task<PagedResult<BookSearchItemDto>> AdvancedSearchAsync(ReaderBookSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.AsNoTracking().AsQueryable();


        // KEYWORD


        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(book => book.Title.Contains(keyword));
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
            query = query.Where(book => book.PublicationYear == request.PublicationYear.Value);
        }


        // AVAILABILITY STATUS


        switch (request.Status?.ToLowerInvariant())
        {
            case "available":

                query = query.Where(
                    book => book.BookCopies.Any(
                        copy => copy.Status.ToString() == "AVAILABLE"));

                break;

            case "unavailable":

                query = query.Where(
                    book => !book.BookCopies.Any(
                        copy => copy.Status.ToString() == "AVAILABLE"));

                break;
        }


        // SORTING


        query = request.Sort?.ToLowerInvariant() switch
        {
            "title" =>
                query.OrderBy(book => book.Title),

            "year" =>
                query
                    .OrderByDescending(book => book.PublicationYear)
                    .ThenBy(book => book.Title),

            "newest" =>
                query.OrderByDescending(book => book.CreatedAt),

            _ =>
                query.OrderBy(book => book.Title)
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
            .Select(book => new BookSearchItemDto
            {
                Id = book.Id,

                Title = book.Title,

                PublicationYear = book.PublicationYear,

                DocumentTypeName = book.DocumentType.Name,

                PublisherName = book.Publisher != null
                    ? book.Publisher.Name
                    : null,

                Authors = book.BookAuthors
                    .Select(ba => ba.Author.Name)
                    .ToList(),

                Categories = book.BookCategories
                    .Select(bc => bc.Category.Name)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<BookSearchItemDto>
        {
            Items = items,

            TotalItems = totalCount,

            Page = page,

            PageSize = pageSize
        };
    }
}