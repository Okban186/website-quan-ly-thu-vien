using WebsiteQuanLyThuVien.Data;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.Services.Storage;
using WebsiteQuanLyThuVien.Exceptions;

namespace WebsiteQuanLyThuVien.Services;

public class BorrowCartService : IBorrowCartService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public BorrowCartService(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    private async Task<Guid> GetMemberIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var memberId = await _context.LibraryMembers
      .Where(m => m.UserId == userId)
      .Select(m => (Guid?)m.Id)
      .FirstOrDefaultAsync();

        if (!memberId.HasValue)
            throw new BusinessException("Không tìm thấy thông tin thành viên liên kết với tài khoản này.");

        return memberId.Value;
    }

    public async Task<PagedResult<BorrowCartItemResponseDto>> GetCartAsync(Guid userId, string? search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var memberId = await GetMemberIdByUserIdAsync(userId);

        var queryable = _context.BorrowCartItems
          .Include(bc => bc.Resource)
              .ThenInclude(r => r.ResourceAuthors)
                  .ThenInclude(ra => ra.Author)
          .Include(bc => bc.Resource)
              .ThenInclude(r => r.ResourceImages)
                  .ThenInclude(ri => ri.File)
          .Where(bc => bc.MemberId == memberId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            queryable = queryable.Where(bc =>
                bc.Resource.Title.ToLower().Contains(keyword) ||
                bc.Resource.ResourceAuthors.Any(ra => ra.Author.Name.ToLower().Contains(keyword))
            );
        }

        var totalItems = await queryable.CountAsync(cancellationToken);

        var cartItems = await queryable
            .OrderByDescending(bc => bc.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(bc => bc.Resource)
                .ThenInclude(r => r.ResourceAuthors)
                    .ThenInclude(ra => ra.Author)
            .Include(bc => bc.Resource)
                .ThenInclude(r => r.ResourceImages)
                    .ThenInclude(ri => ri.File)
            .ToListAsync(cancellationToken);

        var items = new List<BorrowCartItemResponseDto>();
        var activeStatuses = new[] { "PENDING", "APPROVED", "READY_FOR_PICKUP", "DELIVERING" };

        foreach (var bc in cartItems)
        {
            var resource = bc.Resource;

            var authorNames = resource.ResourceAuthors?
                .Select(ra => ra.Author?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList() ?? new List<string>();
            string fullAuthors = string.Join(", ", authorNames);

            string? coverImageUrl = null;
            var primaryImage = resource.ResourceImages?.FirstOrDefault(ri => ri.IsPrimary)
                               ?? resource.ResourceImages?.OrderBy(ri => ri.DisplayOrder).FirstOrDefault();

            if (primaryImage?.File != null)
            {
                coverImageUrl = await _fileStorageService.CreatePublicGetUrlAsync(primaryImage.File.ObjectKey);
            }

            // Đếm số lượng bản sao đang AVAILABLE thực tế trong kho
            int availablePhysicalCount = await _context.LibraryItems
                .CountAsync(li => li.ResourceId == bc.ResourceId
                               && li.Status == LibraryItemStatus.AVAILABLE, cancellationToken);

            //Đếm số lượng request đang active của TẤT CẢ mọi người cho tài liệu này (những request chưa được gán cứng LibraryItemId cụ thể)
            int pendingRequestsCount = await _context.BorrowRequestItems
                .Include(bri => bri.BorrowRequest)
                .CountAsync(bri => bri.ResourceId == bc.ResourceId
                               && bri.LibraryItemId == null // Chưa được thủ kho gán bản sách cụ thể
                               && activeStatuses.Contains(bri.BorrowRequest.Status.ToString()), cancellationToken);

            //Hạn mức khả dụng thực tế sau khi trừ đi các yêu cầu đang chờ xử lý của người khác
            int netAvailableQuota = availablePhysicalCount - pendingRequestsCount;

            //Kiểm tra xem chính user này đã có request chờ cho resource này chưa
            var hasMyExistingRequest = await _context.BorrowRequestItems
                .Include(bri => bri.BorrowRequest)
                .AnyAsync(bri => bri.BorrowRequest.MemberId == memberId
                               && bri.ResourceId == bc.ResourceId
                               && activeStatuses.Contains(bri.BorrowRequest.Status.ToString()), cancellationToken);

            bool isAvailable = netAvailableQuota > 0 && resource.Status == ResourceStatus.ACTIVE && !hasMyExistingRequest;

            string? unavailableReason = null;
            if (hasMyExistingRequest)
            {
                unavailableReason = "Bạn đang có yêu cầu chờ xử lý cho tài liệu này";
            }
            else if (resource.Status != ResourceStatus.ACTIVE)
            {
                unavailableReason = "Tài liệu đã ngừng cung cấp";
            }
            else if (netAvailableQuota <= 0)
            {
                unavailableReason = "Đã hết bản sao khả dụng (Đang có người khác đặt chờ duyệt)";
            }

            items.Add(new BorrowCartItemResponseDto
            {
                ResourceId = bc.ResourceId,
                Title = resource.Title,
                CoverImage = coverImageUrl,
                Authors = fullAuthors,
                AddedAt = bc.CreatedAt,
                IsAvailable = isAvailable,
                UnavailableReason = unavailableReason
            });
        }

        return new PagedResult<BorrowCartItemResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task AddToCartAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken = default)
    {
        var memberId = await GetMemberIdByUserIdAsync(userId);
        var resourceExists = await _context.Resources
            .AnyAsync(r => r.Id == resourceId && r.Status == ResourceStatus.ACTIVE);

        if (!resourceExists)
            throw new BusinessException("Tài liệu không tồn tại hoặc đã ngừng phục vụ.");

        var exists = await _context.BorrowCartItems
            .AnyAsync(bc => bc.MemberId == memberId && bc.ResourceId == resourceId);

        if (exists) throw new BusinessException("Tài liệu đã được thêm vào danh sách chờ", StatusCodes.Status409Conflict);

        var newItem = new BorrowCartItem
        {
            MemberId = memberId,
            ResourceId = resourceId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BorrowCartItems.Add(newItem);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(Guid userId, List<Guid> resourceIds, CancellationToken cancellationToken = default)
    {

        var memberId = await GetMemberIdByUserIdAsync(userId);


        var itemsToDelete = _context.BorrowCartItems.Where(bc => bc.MemberId == memberId && resourceIds.Contains(bc.ResourceId));


        await itemsToDelete.ExecuteDeleteAsync(cancellationToken);
    }


    public async Task SyncCartAsync(Guid userId, List<Guid> resourceIds, CancellationToken cancellationToken = default)
    {
        if (resourceIds == null || !resourceIds.Any()) return;

        var memberId = await GetMemberIdByUserIdAsync(userId);

        var existingResourceIds = await _context.BorrowCartItems
            .Where(bc => bc.MemberId == memberId)
            .Select(bc => bc.ResourceId)
            .ToListAsync();

        var newResourceIds = resourceIds.Distinct().Except(existingResourceIds).ToList();

        if (newResourceIds.Any())
        {
            var validResourceIds = await _context.Resources
                .Where(r => newResourceIds.Contains(r.Id) && r.Status == ResourceStatus.ACTIVE)
                .Select(r => r.Id)
                .ToListAsync();

            foreach (var resId in validResourceIds)
            {
                _context.BorrowCartItems.Add(new BorrowCartItem
                {
                    MemberId = memberId,
                    ResourceId = resId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var memberId = await GetMemberIdByUserIdAsync(userId);

        var items = await _context.BorrowCartItems
            .Where(bc => bc.MemberId == memberId)
            .ToListAsync();

        if (items.Any())
        {
            _context.BorrowCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }

}