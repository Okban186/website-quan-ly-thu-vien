using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;

namespace WebsiteQuanLyThuVien.Controllers.Mvc;

public class BookController : Controller
{

    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }
    public IActionResult Search()
    {
        return View();
    }
}