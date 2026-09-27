using WebsiteQuanLyThuVien.ViewModels;

namespace WebsiteQuanLyThuVien.Services;
public interface IHomeService { 
    Task<HomeViewModel> GetHomeAsync( CancellationToken cancellationToken = default);
}