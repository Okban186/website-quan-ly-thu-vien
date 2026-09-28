using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;


namespace WebsiteQuanLyThuVien.Repositories;

public interface IBookRepository
{
    Task<PagedResult<BookSearchItemDto>> AdvancedSearchAsync(ReaderBookSearchRequest request, CancellationToken cancellationToken = default);

}