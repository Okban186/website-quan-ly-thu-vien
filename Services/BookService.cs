using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Repositories;

namespace WebsiteQuanLyThuVien.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<PagedResult<BookSearchItemDto>> SearchAsync(BookSearchRequest request,CancellationToken cancellationToken)
    {
        request.Keyword = request.Keyword?.Trim();

        if (request.Page < 1)
        {
            request.Page = 1;
        }

        request.PageSize = 12;


        var result = await _bookRepository.SearchAsync(request,cancellationToken);

        return result;
    }


}