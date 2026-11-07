using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.DTOs.ViewModels;

namespace WebsiteQuanLyThuVien.Controllers.Mvc;

public class ResourceController : Controller
{

    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }
    public IActionResult Search()
    {
        return View();
    }


    public async Task<IActionResult> Detail(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        ResourceDetailViewModel viewModel = await _resourceService.getResourceDetail(id);

        if (viewModel == null) return NotFound();



        return View(viewModel);
    }
}