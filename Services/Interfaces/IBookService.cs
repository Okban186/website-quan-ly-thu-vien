using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;

public interface IBookService
{
    Task<PagedResult<BookSearchItemDto>> SearchAsync(
        BookSearchRequest request,
        CancellationToken cancellationToken = default);

    
}

