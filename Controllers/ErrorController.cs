using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.ViewModels;
using WebsiteQuanLyThuVien.Exceptions;

namespace WebsiteQuanLyThuVien.Controllers;

[AllowAnonymous]
[Route("Error")]
public class ErrorController : Controller
{
    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Index()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        var exception = feature?.Error;

        var statusCode = StatusCodes.Status500InternalServerError;

        var title = "Đã xảy ra lỗi";

        var message = "Hệ thống đang gặp sự cố. Vui lòng thử lại sau.";

        if (exception is BusinessException businessException)
        {
            statusCode = businessException.StatusCode;
            title = "Không thể thực hiện yêu cầu";
            message = businessException.Message;
        }

        Response.StatusCode = statusCode;

        var model = new ErrorViewModel
        {
            StatusCode = statusCode,
            Title = title,
            Message = message,
            TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View(model);
    }
}
