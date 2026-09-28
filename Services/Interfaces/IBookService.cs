using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;

public interface IBookService
{
    Task<PagedResult<BookSearchItemDto>> AdvancedSearchAsync(
        ReaderBookSearchRequest request,
        CancellationToken cancellationToken = default);


}

