using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Repositories;

public interface IBookRepository
{
    Task<PagedResult<BookSearchItemDto>> SearchAsync(
        BookSearchRequest request,
        CancellationToken cancellationToken = default);

}