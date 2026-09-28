using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.ViewModels;

namespace WebsiteQuanLyThuVien.Services;

public class HomeService : IHomeService
{
    private readonly ICategoryRepository _categoryRepository;

    public HomeService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<HomeViewModel> GetHomeAsync(CancellationToken cancellationToken = default)
    {
        var categoriesTask = _categoryRepository.GetPagedAsync("", "", 1, 8);

        await Task.WhenAll(categoriesTask);

        PagedResult<CategoryListItem> categoryPage = await categoriesTask;

        var categoryTemporaryList = new List<CategoryViewModel>();


        return new HomeViewModel
        {
            Categories = categoryPage.Items.Select(x => new CategoryViewModel
            {
                Id = x.Id,
                Name = x.Name,
                ResourceCount = x.ResourceCount
            })
            .ToList(),

        };
    }
}
