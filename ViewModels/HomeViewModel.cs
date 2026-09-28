
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.ViewModels;

public class HomeViewModel
{
    public IReadOnlyList<CategoryViewModel> Categories { get; set; } = [];

    public IReadOnlyList<ResourceViewModel> FeaturedResources { get; set; } = [];

    public IReadOnlyList<ResourceViewModel> LatestResources { get; set; } = [];
}

