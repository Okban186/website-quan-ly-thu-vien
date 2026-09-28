using System.Linq.Expressions;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Định nghĩa cách một loại filter xây dựng Expression
/// để áp dụng vào Book query.
///
/// Mỗi handler chịu trách nhiệm cho một field cụ thể.
///
/// Ví dụ:
/// - AuthorFilterHandler → authors
/// - CategoryFilterHandler → categories
/// - CopyCountFilterHandler → copy_count
/// </summary>
public interface IBookFilterHandler
{
    /// <summary>
    /// Tên field mà handler chịu trách nhiệm xử lý.
    ///
    /// Ví dụ:
    /// "authors"
    /// "categories"
    /// "copy_count"
    /// </summary>
    string Field { get; }

    /// <summary>
    /// Xây dựng Expression cho một điều kiện filter.
    /// </summary>
    /// <param name="condition">
    /// Điều kiện filter đã được parser phân tích.
    /// </param>
    /// <param name="parameter">
    /// Parameter đại diện cho Book trong Expression.
    /// </param>
    /// <param name="cancellationToken">
    /// Token dùng để hủy truy vấn database nếu handler cần truy vấn DB.
    /// </param>
    Task<Expression> BuildExpressionAsync(FilterCondition condition, ParameterExpression parameter, CancellationToken cancellationToken = default);
}