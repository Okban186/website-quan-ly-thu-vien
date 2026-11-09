using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Responses;
using WebsiteQuanLyThuVien.DTOs.ViewModels;

public interface IResourceService
{
    Task<PagedResult<ResourceSearchItemDto>> AdvancedSearchAsync(ReaderResourceSearchRequest request, CancellationToken cancellationToken = default);

    Task<ResourceDetailViewModel>? getResourceDetail(Guid? userId, string id, CancellationToken cancellationToken = default);

    Task<ResourceCopiesResponse?> GetResourceCopiesAsync(Guid resourceId, int page = 1, int pageSize = 5, CancellationToken cancellationToken = default);

    Task<ResourceSuggestionResponse> GetSuggestionsAsync(Guid resourceId, CancellationToken cancellationToken = default);
}

