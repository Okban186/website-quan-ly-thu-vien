using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.DTOs.ViewModels;
using WebsiteQuanLyThuVien.Exceptions;

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

        Guid? userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"));

        ResourceDetailViewModel viewModel = await _resourceService.getResourceDetail(userId, id);

        if (viewModel == null) throw new BusinessException("Khong tim thay", StatusCodes.Status404NotFound);



        return View(viewModel);
    }
}