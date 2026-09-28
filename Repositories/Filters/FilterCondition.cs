namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Đại diện cho một điều kiện filter đơn trong FilterExpression.
///
/// Ví dụ:
///
/// authors:("Nguyễn Du")
///
/// sẽ được biểu diễn thành:
///
/// Field = "authors"
/// Operator = "="
/// Value = "Nguyễn Du"
/// </summary>
public sealed record FilterCondition(string Field, string Operator, string Value);