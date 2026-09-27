
namespace WebsiteQuanLyThuVien.ViewModels;

public class HomeViewModel
{
    public IReadOnlyList<CategoryViewModel> Categories { get; set; }
        = [];

    public IReadOnlyList<BookViewModel> FeaturedBooks { get; set; }
        = [];

    public IReadOnlyList<BookViewModel> LatestBooks { get; set; }
        = [];
}

