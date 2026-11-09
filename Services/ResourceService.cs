using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.DTOs.Responses;
using WebsiteQuanLyThuVien.DTOs.ViewModels;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.Services.Storage;

namespace WebsiteQuanLyThuVien.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly ApplicationDbContext _context;

    private readonly IFileStorageService _fileStorageService;

    public ResourceService(IResourceRepository resourceRepository, ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _resourceRepository = resourceRepository;
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<PagedResult<ResourceSearchItemDto>> AdvancedSearchAsync(ReaderResourceSearchRequest request, CancellationToken cancellationToken)
    {
        request.Keyword = request.Keyword?.Trim();

        if (request.Page < 1)
        {
            request.Page = 1;
        }

        request.PageSize = 12;


        var result = await _resourceRepository.AdvancedSearchAsync(request, cancellationToken);

        return result;
    }

    public async Task<ResourceDetailViewModel?> getResourceDetail(Guid? userId, string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out Guid fid))
            return null;

        var resource = await _context.Resources
            .Include(c => c.ResourceCategories)
                .ThenInclude(rc => rc.Category)
            .Include(au => au.ResourceAuthors)
                .ThenInclude(ra => ra.Author)
            .Include(dc => dc.DocumentType)
            .Include(pl => pl.Publisher)
            .FirstOrDefaultAsync(r => r.Id == fid, cancellationToken);

        if (resource == null)
            return null;

        var resourceImages = await _context.ResourceImages
            .Include(r => r.File)
            .Where(ri => ri.ResourceId == fid)
            .ToListAsync(cancellationToken);

        List<ResourceDetailViewModel.ImageResourceItem> Images =
            (await Task.WhenAll(resourceImages.Select(async ri =>
                new ResourceDetailViewModel.ImageResourceItem(
                    await _fileStorageService.CreatePublicGetUrlAsync(ri.File.ObjectKey),
                    ri.DisplayOrder,
                    ri.IsPrimary
                )
            ))).ToList();

        List<string> categoryNames = resource.ResourceCategories
            .Select(rc => rc.Category.Name)
            .ToList();

        List<string> authorNames = resource.ResourceAuthors
            .Select(ra => ra.Author.Name)
            .ToList();

        int totalCopies = await _context.LibraryItems
            .CountAsync(lb => lb.ResourceId == fid, cancellationToken);

        var memberId = await _context.LibraryMembers
        .Where(m => m.UserId == userId)
        .Select(m => m.Id)
        .FirstOrDefaultAsync(cancellationToken);

        bool isInCart = userId == null ? false : await _context.BorrowCartItems
            .AnyAsync(bc => bc.MemberId == memberId && bc.ResourceId == resource.Id);

        var phone = "";
        var address = "";
        bool isBorrowing = false;
        if (userId != null)
        {
            var dynam = await _context.Users.Where(u => u.Id == userId).Select(u => new { u.Phone, u.Address }).FirstOrDefaultAsync();

            phone = dynam?.Phone ?? "";
            address = dynam?.Address ?? "";


            if (userId.HasValue)
            {
                var activeStatuses = new[] {
        BorrowRequestStatus.PENDING,
        BorrowRequestStatus.APPROVED,
        BorrowRequestStatus.READY_FOR_PICKUP,
        BorrowRequestStatus.DELIVERING
    };

                // Kiểm tra xem user có đang có request chờ xử lý/đang mượn đối với tài liệu này không
                bool hasActiveRequest = await _context.BorrowRequestItems
                    .Include(bri => bri.BorrowRequest)
                    .AnyAsync(bri => bri.BorrowRequest.MemberId == memberId
                                  && bri.ResourceId == fid
                                  && activeStatuses.Contains(bri.BorrowRequest.Status));

                bool hasActiveLoan = await _context.LoanItems
                    .Include(li => li.Loan)
                    .Include(li => li.LibraryItem)
                    .AnyAsync(li => li.Loan.MemberId == memberId
                                 && li.LibraryItem.ResourceId == fid
                                 && li.Loan.Status == LoanStatus.ACTIVE
                                 && li.Status == LoanItemStatus.BORROWED);

                isBorrowing = hasActiveRequest || hasActiveLoan;
            }
        }

        int availableCopies = await _context.LibraryItems.CountAsync(lb => lb.ResourceId == fid && lb.Status == LibraryItemStatus.AVAILABLE, cancellationToken);

        return new ResourceDetailViewModel
        {
            Id = resource.Id,
            Title = resource.Title,
            Images = Images,
            Phone = phone,
            Address = address,
            PhysicalDescription = resource.PhysicalDescription!,
            ISBN = resource.Isbn!,
            Categories = categoryNames,
            Authors = authorNames,
            IsInCart = isInCart,
            DocumentTypeName = resource.DocumentType.Name,
            Description = resource.Description,
            PublisherName = resource.Publisher.Name,
            PublicationYear = resource.PublicationYear,
            AvailableCopies = availableCopies,
            TotalCopies = totalCopies,
            IsBorrowing = isBorrowing
        };


    }

    public async Task<ResourceCopiesResponse?> GetResourceCopiesAsync(Guid resourceId, int page = 1, int pageSize = 5, CancellationToken cancellationToken = default)
    {

        if (page < 1)
            page = 1;

        var query = _context.LibraryItems
            .AsNoTracking()
            .Where(x =>
                x.ResourceId == resourceId &&
                x.Status != LibraryItemStatus.REMOVED);

        int totalItems = await query.CountAsync(cancellationToken);

        // Thống kê 3 nhóm trạng thái
        var statistics = await query
            .GroupBy(x => 1)
            .Select(g => new
            {
                Available = g.Count(x =>
                    x.Status == LibraryItemStatus.AVAILABLE),

                Borrowing = g.Count(x =>
                    x.Status == LibraryItemStatus.RESERVED ||
                    x.Status == LibraryItemStatus.BORROWD),

                Processing = g.Count(x =>
                    x.Status == LibraryItemStatus.RETURNED_PENDING_CHECK ||
                    x.Status == LibraryItemStatus.LOST ||
                    x.Status == LibraryItemStatus.DAMAGED ||
                    x.Status == LibraryItemStatus.MAINTENANCE)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var copies = await query
            .OrderBy(x => x.Barcode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ResourceCopiesResponse.ResourceCopyItem
            {
                Barcode = x.Barcode,
                Status = x.Status,
                LocationName = x.Location != null
                    ? x.Location.Name
                    : null
            })
            .ToListAsync(cancellationToken);

        return new ResourceCopiesResponse
        {
            AvailableCopies = statistics?.Available ?? 0,
            BorrowingCopies = statistics?.Borrowing ?? 0,
            ProcessingCopies = statistics?.Processing ?? 0,

            Copies = new PagedResult<ResourceCopiesResponse.ResourceCopyItem>
            {
                Items = copies,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            }
        };
    }


    public async Task<ResourceSuggestionResponse> GetSuggestionsAsync(Guid resourceId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Resources.AnyAsync(x => x.Id == resourceId, cancellationToken);

        if (!exists)
            throw new BusinessException("Id này không tồn tại");

        var categoryIds = await _context.ResourceCategories
            .Where(rc => rc.ResourceId == resourceId)
            .Select(rc => rc.CategoryId)
            .ToListAsync(cancellationToken);

        if (categoryIds.Count == 0)
            return new ResourceSuggestionResponse();

        var rawSuggestions = await _context.Resources
            .AsNoTracking()
            .Where(r =>
                r.Id != resourceId &&
                r.ResourceCategories.Any(rc =>
                    categoryIds.Contains(rc.CategoryId)))
            .Select(r => new
            {
                r.Id,
                r.Title,
                PublishYear = r.PublicationYear ?? 0,
                MatchingCategoryCount = r.ResourceCategories
                    .Count(rc => categoryIds.Contains(rc.CategoryId)),

                Categories = r.ResourceCategories
                    .Select(rc => rc.Category.Name)
                    .ToList(),

                Authors = r.ResourceAuthors
                    .Select(ra => ra.Author.Name)
                    .ToList(),

                ObjectKey = r.ResourceImages
                    .OrderByDescending(img => img.IsPrimary)
                    .ThenBy(img => img.DisplayOrder)
                    .Select(img => img.File != null ? img.File.ObjectKey : null)
                    .FirstOrDefault()
            })
            .OrderByDescending(x => x.MatchingCategoryCount)
            .ThenBy(x => x.Title)
            .Take(5)
            .ToListAsync(cancellationToken);

        if (rawSuggestions.Count == 0)
            return new ResourceSuggestionResponse();


        var suggestionIds = rawSuggestions.Select(x => x.Id).ToList();

        var availableCopiesDict = await _context.LibraryItems
            .AsNoTracking()
            .Where(li => suggestionIds.Contains(li.ResourceId) && li.Status == LibraryItemStatus.AVAILABLE)
            .GroupBy(li => li.ResourceId)
            .Select(g => new { ResourceId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ResourceId, x => x.Count, cancellationToken);

        var suggestions = await Task.WhenAll(rawSuggestions.Select(async x => new ResourceSuggestionResponse.ResourceSuggestionItem
        {
            Id = x.Id,
            Title = x.Title,
            Categories = x.Categories,
            Authors = x.Authors,
            PublishYear = x.PublishYear,
            AvailableCopies = availableCopiesDict.TryGetValue(x.Id, out var count) ? count : 0,

            CoverUrl = !string.IsNullOrEmpty(x.ObjectKey)
                ? await _fileStorageService.CreatePublicGetUrlAsync(x.ObjectKey)
                : null
        }));

        return new ResourceSuggestionResponse { Items = suggestions.ToList() };
    }
}