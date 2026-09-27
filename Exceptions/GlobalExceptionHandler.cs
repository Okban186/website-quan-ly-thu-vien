using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyThuVien.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Xử lý ngoại lệ nếu dùng fetch để gọi tức là application/json thì gửi lỗi qua json không thì qua Error.cshtml
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();

        var originalPath = exceptionFeature?.Path;

        var isApiRequest = originalPath?.StartsWith("/api") == true;
        // MVC request → để UseExceptionHandler("/Error")
        // chuyển request sang ErrorController.
        if (!isApiRequest)
        {
            return false;
        }

        _logger.LogError(
            exception,
            "Unhandled exception. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        var statusCode =
            StatusCodes.Status500InternalServerError;

        var title = "Đã xảy ra lỗi";

        var detail = "Hệ thống đang gặp sự cố. Vui lòng thử lại sau.";

        if (exception is BusinessException businessException)
        {
            statusCode = businessException.StatusCode;
            title = "Không thể thực hiện yêu cầu";
            detail = businessException.Message;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
