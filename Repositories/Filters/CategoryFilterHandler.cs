using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Xử lý filter theo thể loại sách.
///
/// Field:
/// categories
/// </summary>
public sealed class CategoryFilterHandler : IResourceFilterHandler
{
    private readonly ApplicationDbContext _context;

    public string Field => "categories";

    public CategoryFilterHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xây điều kiện kiểm tra resource có thuộc category được chỉ định hay không.
    ///
    /// Expression tương đương:
    ///
    /// resource => resource.resourceCategories.Any(
    ///     rc => rc.CategoryId == categoryId)
    /// </summary>
    public async Task<Expression> BuildExpressionAsync(FilterCondition condition, ParameterExpression parameter, CancellationToken cancellationToken = default)
    {
        if (condition.Operator != "=")
        {
            throw new ArgumentException("categories chỉ hỗ trợ toán tử '='.");
        }

        var categoryId = await _context.Categories
            .Where(x => x.Name == condition.Value)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!categoryId.HasValue)
        {
            return Expression.Constant(false);
        }

        var resourceCategories = Expression.Property(parameter, nameof(Resource.ResourceCategories));

        var categoryParameter = Expression.Parameter(typeof(ResourceCategory), "bc");

        var categoryIdProperty = Expression.Property(categoryParameter, nameof(ResourceCategory.CategoryId));

        var equals = Expression.Equal(categoryIdProperty, Expression.Constant(categoryId.Value));

        var predicate = Expression.Lambda<Func<ResourceCategory, bool>>(equals, categoryParameter);

        var anyMethod = typeof(Enumerable)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(Enumerable.Any) &&
                method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(ResourceCategory));

        return Expression.Call(anyMethod, resourceCategories, predicate);
    }
}