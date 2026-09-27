using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Controllers.Api;

[Route("api/books")]
public class BookController : Controller
{

    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    [Route("search")]
    [AllowAnonymous]
    public async Task<IActionResult> BooksSearchAsync([FromBody] BookSearchRequest request)
    {
        var result = await _bookService.SearchAsync(request);

        return Ok(result);
    }
}