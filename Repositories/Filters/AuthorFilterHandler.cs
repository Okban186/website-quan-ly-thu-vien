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
public sealed class AuthorFilterHandler : IResourceFilterHandler
{
    private readonly ApplicationDbContext _context;

    public string Field => "authors";

    public AuthorFilterHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xây điều kiện kiểm tra Resource có tác giả được chỉ định hay không.
    ///
    /// Expression tương đương:
    ///
    /// Resource => Resource.ResourceAuthors.Any(
    ///     ra => ra.AuthorId == authorId)
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

        // resource.ResourceAuthors
        var resourceAuthors = Expression.Property(
            parameter,
            nameof(Resource.ResourceAuthors));

        // ra (tên tắt thay cho resource author)
        var authorParameter = Expression.Parameter(typeof(ResourceAuthor), "ba");

        // ra.AuthorId
        var authorIdProperty = Expression.Property(authorParameter, nameof(ResourceAuthor.AuthorId));

        // ra.AuthorId == authorId
        var equals = Expression.Equal(authorIdProperty, Expression.Constant(authorId.Value));

        // ra => ra.AuthorId == authorId
        var predicate = Expression.Lambda<Func<ResourceAuthor, bool>>(equals, authorParameter);

        // resource.ResourceAuthors.Any(...)
        var anyMethod = typeof(Enumerable)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(Enumerable.Any) &&
                method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(ResourceAuthor));

        return Expression.Call(
            anyMethod,
            resourceAuthors,
            predicate);
    }
}