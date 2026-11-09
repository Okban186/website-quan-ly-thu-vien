using WebsiteQuanLyThuVien.DTOs;

namespace WebsiteQuanLyThuVien.Services;

public interface IBorrowCartService
{
    Task<PagedResult<BorrowCartItemResponseDto>> GetCartAsync(Guid userId, string? search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task AddToCartAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken = default);
    Task RemoveFromCartAsync(Guid userId, List<Guid> resourceIds, CancellationToken cancellationToken = default);
    Task SyncCartAsync(Guid userId, List<Guid> resourceIds, CancellationToken cancellationToken = default);
    Task ClearCartAsync(Guid userId, CancellationToken cancellationToken = default);
}