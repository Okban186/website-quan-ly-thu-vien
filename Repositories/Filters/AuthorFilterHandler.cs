using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Xử lý filter theo tác giả.
///
/// Field:
/// authors
/// </summary>
public sealed class AuthorFilterHandler : IBookFilterHandler
{
    private readonly ApplicationDbContext _context;

    public string Field => "authors";

    public AuthorFilterHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xây điều kiện kiểm tra Book có tác giả được chỉ định hay không.
    ///
    /// Expression tương đương:
    ///
    /// book => book.BookAuthors.Any(
    ///     ba => ba.AuthorId == authorId)
    /// 
    /// 
    /// </summary>
    public async Task<Expression> BuildExpressionAsync(FilterCondition condition, ParameterExpression parameter, CancellationToken cancellationToken = default)
    {
        if (condition.Operator != "=")
        {
            throw new BusinessException("authors chỉ hỗ trợ toán tử '='.");
        }

        // Resolve tên tác giả → AuthorId.
        var authorId = await _context.Authors
            .Where(x => x.Name == condition.Value)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        // Tác giả không tồn tại → điều kiện luôn false.
        if (!authorId.HasValue)
        {
            return Expression.Constant(false);
        }

        // book.BookAuthors
        var bookAuthors = Expression.Property(
            parameter,
            nameof(Book.BookAuthors));

        // ba (tên tắt thay cho book author)
        var authorParameter = Expression.Parameter(typeof(BookAuthor), "ba");

        // ba.AuthorId
        var authorIdProperty = Expression.Property(authorParameter, nameof(BookAuthor.AuthorId));

        // ba.AuthorId == authorId
        var equals = Expression.Equal(authorIdProperty, Expression.Constant(authorId.Value));

        // ba => ba.AuthorId == authorId
        var predicate = Expression.Lambda<Func<BookAuthor, bool>>(equals, authorParameter);

        // book.BookAuthors.Any(...)
        var anyMethod = typeof(Enumerable)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(Enumerable.Any) &&
                method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(BookAuthor));

        return Expression.Call(
            anyMethod,
            bookAuthors,
            predicate);
    }
}