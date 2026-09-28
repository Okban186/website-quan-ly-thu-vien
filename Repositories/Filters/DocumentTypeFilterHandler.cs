using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Xử lý filter theo loại tài liệu.
///
/// Field:
/// document_types
/// </summary>
public sealed class DocumentTypeFilterHandler : IResourceFilterHandler
{
    private readonly ApplicationDbContext _context;

    public string Field => "document_types";

    public DocumentTypeFilterHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xây điều kiện:
    ///
    /// resource => resource.DocumentTypeId == documentTypeId
    /// </summary>
    public async Task<Expression> BuildExpressionAsync(FilterCondition condition, ParameterExpression parameter, CancellationToken cancellationToken = default)
    {
        if (condition.Operator != "=")
        {
            throw new ArgumentException("document_types chỉ hỗ trợ toán tử '='.");
        }

        var documentTypeId = await _context.DocumentTypes
            .Where(x => x.Name == condition.Value)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!documentTypeId.HasValue)
        {
            return Expression.Constant(false);
        }

        var documentTypeIdProperty = Expression.Property(parameter, nameof(Resource.DocumentTypeId));

        var documentTypeIdValue = Expression.Constant(documentTypeId.Value, typeof(Guid?));

        return Expression.Equal(documentTypeIdProperty, documentTypeIdValue);
    }
}