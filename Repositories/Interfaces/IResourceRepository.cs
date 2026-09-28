using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;


namespace WebsiteQuanLyThuVien.Repositories;

public interface IResourceRepository
{
    Task<PagedResult<ResourceSearchItemDto>> AdvancedSearchAsync(ReaderResourceSearchRequest request, CancellationToken cancellationToken = default);

}