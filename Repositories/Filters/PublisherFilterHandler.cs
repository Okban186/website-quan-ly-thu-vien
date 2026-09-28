using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Xử lý filter theo nhà xuất bản.
///
/// Field:
/// publishers
/// </summary>
public sealed class PublisherFilterHandler : IResourceFilterHandler
{
    private readonly ApplicationDbContext _context;

    public string Field => "publishers";

    public PublisherFilterHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xây điều kiện:
    ///
    /// resource => resource.PublisherId == publisherId
    /// </summary>
    public async Task<Expression> BuildExpressionAsync(FilterCondition condition, ParameterExpression parameter, CancellationToken cancellationToken = default)
    {
        if (condition.Operator != "=")
        {
            throw new ArgumentException("publishers chỉ hỗ trợ toán tử '='.");
        }

        var publisherId = await _context.Publishers
            .Where(x => x.Name == condition.Value)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!publisherId.HasValue)
        {
            return Expression.Constant(false);
        }

        var publisherIdProperty = Expression.Property(parameter, nameof(Resource.PublisherId));

        var publisherIdValue = Expression.Constant(publisherId.Value, typeof(Guid?));

        return Expression.Equal(publisherIdProperty, publisherIdValue);
    }
}