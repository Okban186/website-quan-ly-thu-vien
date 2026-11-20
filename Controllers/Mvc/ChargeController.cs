using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyThuVien.Controllers.Mvc;

[AuthorizeExceptRole("ADMIN")]
[Route("charges")]
public class ChargeController : Controller
{

    [Route("mine")]
    public IActionResult Index()
    {
        return View();
    }
}