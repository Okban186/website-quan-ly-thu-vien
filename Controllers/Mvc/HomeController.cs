using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Services;
using WebsiteQuanLyThuVien.ViewModels;

namespace WebsiteQuanLyThuVien.Controllers;


public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }
    public async Task<IActionResult> Index()
    {
        HomeViewModel homeViewModel = await _homeService.GetHomeAsync();
        return View(homeViewModel);
    }
}