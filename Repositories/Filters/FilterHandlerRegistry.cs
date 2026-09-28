using WebsiteQuanLyThuVien.Exceptions;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Registry quản lý toàn bộ Book filter handler.
///
/// Registry giúp hệ thống tìm handler phù hợp dựa trên tên field
/// mà không cần BookRepository phải biết từng loại filter.
/// </summary>
public sealed class FilterHandlerRegistry
{
    private readonly Dictionary<string, IBookFilterHandler> _handlers;

    public FilterHandlerRegistry(IEnumerable<IBookFilterHandler> handlers)
    {
        _handlers = handlers.ToDictionary(handler => handler.Field, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Lấy handler tương ứng với field.
    /// </summary>
    /// <param name="field">Tên field cần xử lý.</param>
    /// <returns>Handler tương ứng.</returns>
    /// <exception cref="ArgumentException">
    /// Được ném ra nếu field không được hỗ trợ.
    /// </exception>
    public IBookFilterHandler GetHandler(string field)
    {
        if (_handlers.TryGetValue(field, out var handler))
        {
            return handler;
        }

        throw new BusinessException($"Field filter không được hỗ trợ: {field}");
    }

    /// <summary>
    /// Kiểm tra field có handler tương ứng hay không.
    /// </summary>
    public bool HasHandler(string field)
    {
        return _handlers.ContainsKey(field);
    }
}