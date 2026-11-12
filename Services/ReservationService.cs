using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.DTOs.Responses;
using WebsiteQuanLyThuVien.Services.Storage;
using WebsiteQuanLyThuVien.DTOs;

namespace WebsiteQuanLyThuVien.Services;

[AuthorizeExceptRole("ADMIN")]
public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public ReservationService(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task CancelRequestItem(Guid id, CancellationToken cancellationToken = default)
    {
        var requestItem = await _context.BorrowRequestItems
            .Include(bri => bri.BorrowRequest)
            .FirstOrDefaultAsync(bri => bri.Id == id);

        // Kiểm tra xem Item có tồn tại không
        if (requestItem == null || requestItem.BorrowRequest == null)
        {
            throw new BusinessException("Không tìm thấy yêu cầu tài liệu này", StatusCodes.Status404NotFound);
        }

        var mainRequest = requestItem.BorrowRequest;

        //Chặn hủy nếu bản thân Item hoặc Đơn tổng đã hoàn tất/đóng trước đó
        if (requestItem.Status == BorrowRequestItemStatus.CANCELLED ||
            requestItem.Status == BorrowRequestItemStatus.COMPLETED ||
            requestItem.Status == BorrowRequestItemStatus.REJECTED)
        {
            throw new BusinessException("Tài liệu này đã được xử lý hoặc đã hủy trước đó");
        }

        //Dùng BorrowRequest để quyết định xem có cho phép hủy Item này hay không
        if (mainRequest.RequestType == BorrowRequestType.DELIVERY)
        {
            // Đối với GIAO TẬN NƠI: Nếu trạng thái đơn tổng đã là DELIVERING thì KHÔNG ĐƯỢC phép hủy bất kỳ item nào nữa
            if (mainRequest.Status == BorrowRequestStatus.DELIVERING)
            {
                throw new BusinessException("Tài liệu đang trong quá trình vận chuyển, không thể hủy.");
            }
        }
        else if (mainRequest.RequestType == BorrowRequestType.RESERVATION)
        {
            // Đối với NHẬN TẠI THƯ VIỆN: Cho phép hủy tự do ở các trạng thái (PENDING, APPROVED, READY_FOR_PICKUP)
            // Ngoại trừ các trạng thái kết thúc luồng
            var invalidReservationStatuses = new[] {
            BorrowRequestStatus.COMPLETED,
            BorrowRequestStatus.REJECTED,
            BorrowRequestStatus.CANCELLED,
            BorrowRequestStatus.EXPIRED
        };

            if (invalidReservationStatuses.Contains(mainRequest.Status))
            {
                throw new BusinessException("Yêu cầu mượn sách này đã đóng hoặc hoàn tất.");
            }
        }

        //Tiến hành hủy duy nhất Item được chọn
        requestItem.Status = BorrowRequestItemStatus.CANCELLED;
        requestItem.UpdatedAt = DateTime.UtcNow;

        // Kiểm tra xem đơn tổng còn Item nào khác đang hoạt động không
        // Nếu tất cả các item khác trong đơn này đều đã bị hủy, thì tự động hủy luôn cả Đơn tổng (mainRequest)
        bool hasOtherActiveItems = await _context.BorrowRequestItems
            .AnyAsync(bri => bri.BorrowRequestId == mainRequest.Id &&
                             bri.Id != id &&
                             bri.Status != BorrowRequestItemStatus.CANCELLED &&
                             bri.Status != BorrowRequestItemStatus.REJECTED);

        if (!hasOtherActiveItems)
        {
            mainRequest.Status = BorrowRequestStatus.CANCELLED;
            mainRequest.UpdatedAt = DateTime.UtcNow;
        }

        // Lưu thay đổi xuống cơ sở dữ liệu
        await _context.SaveChangesAsync();

    }

    public async Task<BorrowRequestResultResponse> CreateRequestAsync(Guid userId, CreateBorrowRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.RequestType != "RESERVATION" && request.RequestType != "DELIVERY")
        {
            throw new BusinessException("Phương thức không hợp lệ.", StatusCodes.Status400BadRequest);
        }
        if (request.ResourceIds == null || !request.ResourceIds.Any())
        {
            throw new BusinessException("Vui lòng chọn ít nhất một tài liệu để đặt mượn");
        }

        if (request.RequestType == "DELIVERY")
        {
            if (string.IsNullOrWhiteSpace(request.DeliveryAddress) ||
                string.IsNullOrWhiteSpace(request.RecipientPhone))
            {
                throw new BusinessException("Vui lòng cung cấp đầy đủ địa chỉ và số điện thoại người nhận.", StatusCodes.Status400BadRequest);
            }

            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^(0[3|5|7|8|9])+([0-9]{8})$");
            if (!phoneRegex.IsMatch(request.RecipientPhone.Trim()))
            {
                throw new BusinessException("Số điện thoại nhận sách không đúng định dạng (phải gồm 10 chữ số hợp lệ).", StatusCodes.Status400BadRequest);
            }
        }

        var userCardInfo = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                User = u,
                Member = u.LibraryMember,
                Card = u.LibraryMember != null ? u.LibraryMember.LibraryCard : null,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userCardInfo?.Member == null || userCardInfo.Card == null)
        {
            throw new BusinessException("Tài khoản của bạn chưa được kích hoạt hồ sơ thành viên hoặc chưa cấp thẻ thư viện", StatusCodes.Status403Forbidden);
        }

        var memberId = userCardInfo.Member.Id;
        var card = userCardInfo.Card;

        if (userCardInfo.Member.MembershipStatus != LibraryMembershipStatus.ACTIVE)
        {
            throw new BusinessException($"Tư cách thành viên của bạn đang ở trạng thái: {userCardInfo.Member.MembershipStatus}", StatusCodes.Status403Forbidden);
        }

        if (card.Status != LibraryCardStatus.ACTIVE)
        {
            throw new BusinessException($"Thẻ thư viện của bạn không hoạt động (Trạng thái: {card.Status})", StatusCodes.Status403Forbidden);
        }

        if (card.ExpiredAt.HasValue && card.ExpiredAt.Value < DateTime.UtcNow)
        {
            if (card.Status != LibraryCardStatus.EXPIRED)
            {
                card.Status = LibraryCardStatus.EXPIRED;
                card.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            throw new BusinessException("Thẻ thư viện của bạn đã hết hạn, vui lòng gia hạn để tiếp tục", StatusCodes.Status403Forbidden);
        }

        var hasUnpaidCharges = await _context.Charges
            .AnyAsync(c => c.MemberId == memberId && c.Status == ChargeStatus.UNPAID, cancellationToken);
        if (hasUnpaidCharges)
        {
            throw new BusinessException("Bạn đang có khoản phí phạt chưa thanh toán", StatusCodes.Status402PaymentRequired);
        }

        var nowUtc = DateTime.UtcNow;
        var hasOverdueBooks = await _context.LoanItems
            .Include(li => li.Loan)
            .AnyAsync(li => li.Loan.MemberId == memberId && li.Status == LoanItemStatus.BORROWED && li.DueAt < nowUtc, cancellationToken);
        if (hasOverdueBooks)
        {
            throw new BusinessException("Bạn đang có sách mượn đã quá hạn chưa trả", StatusCodes.Status403Forbidden);
        }

        var roles = userCardInfo.Roles;
        int maxAllowedBooks = roles.Contains("LIBRARIAN") || roles.Contains("STOREKEEPER") || roles.Contains("CATALOGER") ? 10 : 5;

        var activeParentStatuses = new[]
        {
            BorrowRequestStatus.PENDING,
            BorrowRequestStatus.APPROVED,
            BorrowRequestStatus.READY_FOR_PICKUP,
            BorrowRequestStatus.DELIVERING
        };

        var inactiveItemStatuses = new[]
        {
            BorrowRequestItemStatus.CANCELLED,
            BorrowRequestItemStatus.REJECTED,
        BorrowRequestItemStatus.EXPIRED
        };


        var totalActiveBooksCount = await _context.BorrowRequestItems
            .AsNoTracking()
            .CountAsync(bri => bri.BorrowRequest.MemberId == memberId
                               && activeParentStatuses.Contains(bri.BorrowRequest.Status)
                               && !inactiveItemStatuses.Contains(bri.Status), cancellationToken)
            + await _context.LoanItems
            .AsNoTracking()
            .CountAsync(li => li.Loan.MemberId == memberId
                               && li.Status == LoanItemStatus.BORROWED, cancellationToken);


        int incomingBooksCount = request.ResourceIds.Distinct().Count();

        if (totalActiveBooksCount + incomingBooksCount > maxAllowedBooks)
        {
            throw new BusinessException($"Yêu cầu thất bại. Tổng số sách đang mượn và đang đặt trước của bạn ({totalActiveBooksCount}) cộng với số lượng đăng ký mới ({incomingBooksCount}) vượt quá giới hạn tối đa cho phép là {maxAllowedBooks} .", StatusCodes.Status403Forbidden);
        }

        foreach (var resourceId in request.ResourceIds.Distinct())
        {
            var resource = await _context.Resources
                .FirstOrDefaultAsync(r => r.Id == resourceId && r.Status == ResourceStatus.ACTIVE, cancellationToken);

            if (resource == null)
            {
                throw new BusinessException($"Tài liệu với ID {resourceId} không tồn tại hoặc đã ngừng cung cấp.");
            }


            var hasPendingRequest = await _context.BorrowRequestItems
                .AsNoTracking()
                .AnyAsync(bri => bri.BorrowRequest.MemberId == memberId
                               && bri.ResourceId == resourceId
                               && activeParentStatuses.Contains(bri.BorrowRequest.Status)
                               && !inactiveItemStatuses.Contains(bri.Status), cancellationToken);

            if (hasPendingRequest)
            {
                throw new BusinessException($"Bạn đang có một yêu cầu chờ xử lý đối với tài liệu: \"{resource.Title}\". Không thể đặt trùng lặp.", StatusCodes.Status409Conflict);
            }

            // Chặn trùng lặp sách đang cầm về nhà (Giữ nguyên)
            var isAlreadyBorrowed = await _context.LoanItems
                .AsNoTracking()
                .AnyAsync(li => li.Loan.MemberId == memberId
                               && li.LibraryItem.ResourceId == resourceId
                               && li.Status == LoanItemStatus.BORROWED, cancellationToken);

            if (isAlreadyBorrowed)
            {
                throw new BusinessException($"Bạn đang mượn tài liệu: \"{resource.Title}\" và chưa hoàn trả. Không thể yêu cầu đặt thêm.", StatusCodes.Status409Conflict);
            }


            int availablePhysicalCount = await _context.LibraryItems
                .AsNoTracking()
                .CountAsync(li => li.ResourceId == resourceId
                               && li.Status == LibraryItemStatus.AVAILABLE, cancellationToken);

            int pendingRequestsCount = await _context.BorrowRequestItems
                .AsNoTracking() // Dùng AsNoTracking cho tối ưu
                .CountAsync(bri => bri.ResourceId == resourceId
                               && bri.LibraryItemId == null
                               && activeParentStatuses.Contains(bri.BorrowRequest.Status)
                               && !inactiveItemStatuses.Contains(bri.Status), cancellationToken);

            int availableQuota = availablePhysicalCount - pendingRequestsCount;

            if (availableQuota <= 0)
            {
                throw new BusinessException($"Tài liệu \"{resource.Title}\" hiện đã hết bản khả dụng trên hệ thống (Đang bảo trì hoặc đã có người đặt trước).", StatusCodes.Status400BadRequest);
            }
        }




        var borrowRequestCode = GenerateRequestCode();

        var borrowRequestId = Guid.NewGuid();
        var borrowRequest = new BorrowRequest
        {
            Id = borrowRequestId,
            MemberId = memberId,
            RequestCode = borrowRequestCode,
            RequestType = Enum.Parse<BorrowRequestType>(request.RequestType),
            Status = BorrowRequestStatus.PENDING,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var resourceId in request.ResourceIds.Distinct())
        {
            borrowRequest.Items.Add(new BorrowRequestItem
            {
                Id = Guid.NewGuid(),
                BorrowRequestId = borrowRequestId,
                ResourceId = resourceId,
                LibraryItemId = null,
                Status = BorrowRequestItemStatus.PENDING,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (request.RequestType == "DELIVERY")
        {

            var delivery = new Delivery
            {
                Id = Guid.NewGuid(),
                BorrowRequestId = borrowRequestId,
                Address = request.DeliveryAddress!,
                RecipientName = userCardInfo.User.FullName,
                RecipientPhone = request.RecipientPhone!,
                Status = DeliveryStatus.PENDING,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Deliveries.Add(delivery);
        }

        _context.BorrowRequests.Add(borrowRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return new BorrowRequestResultResponse
        {
            Id = borrowRequestId,
            RequestCode = borrowRequestCode,
            RequestType = request.RequestType,
        }; ;
    }


    private string GenerateRequestCode()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

        // Lấy 4 byte ngẫu nhiên bằng RandomNumberGenerator an toàn mã hóa
        byte[] randomBytes = new byte[4];
        System.Security.Cryptography.RandomNumberGenerator.Fill(randomBytes);

        // Mã hóa Base32/Hex ngắn gọn (loại bỏ ký tự dễ nhầm lẫn nếu cần)
        string randomPart = Convert.ToHexString(randomBytes)[..6];

        return $"YC-{datePart}-{randomPart}";
    }

    public async Task<PagedResult<BorrowItemListResponse>> GetBorrowItemList(Guid userId, string type, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        // Kiểm tra tính hợp lệ của tham số phân trang
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        string cleanType = type?.Trim().ToLower() ?? "";
        if (cleanType != "borrowed" && cleanType != "history")
            throw new BusinessException("Type không hợp lệ");

        var member = await _context.LibraryMembers.FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
        if (member == null) return new PagedResult<BorrowItemListResponse> { Page = page, PageSize = pageSize };

        Guid memberId = member.Id;
        DateTime now = DateTime.UtcNow;
        var allItems = new List<BorrowItemListResponse>();

        await _context.LoanItems
            .Where(li => li.Loan.MemberId == memberId
                         && li.Status == LoanItemStatus.BORROWED
                         && li.DueAt < now)
            .ExecuteUpdateAsync(s => s
                .SetProperty(li => li.Status, LoanItemStatus.OVERDUE)
                .SetProperty(li => li.UpdatedAt, now), cancellationToken);

        if (cleanType == "borrowed")
        {
            // Nhánh A: Sách đang mượn thực tế trên tay (BORROWED / OVERDUE)
            var activeLoanStatuses = new[] { LoanItemStatus.BORROWED, LoanItemStatus.OVERDUE };

            var borrowedList = await _context.LoanItems
                .Where(li => li.Loan.MemberId == memberId && activeLoanStatuses.Contains(li.Status))
                .Select(li => new
                {
                    LoanItemId = li.Id,
                    ReturnedAt = li.ReturnedAt,
                    ItemLoanStatus = li.Status,
                    BorrowRequestItemId = _context.BorrowRequestItems
                        .Where(bri => bri.BorrowRequestId == li.Loan.BorrowRequestId && bri.ResourceId == li.LibraryItem.ResourceId)
                        .Select(bri => (Guid?)bri.Id)
                        .FirstOrDefault() ?? Guid.Empty,
                    li.DueAt,
                    BorrowedAt = li.Loan.BorrowedAt,
                    Resource = li.LibraryItem.Resource,
                    RenewCount = _context.LoanRenewals.Count(lr => lr.LoanItemId == li.Id),
                    Authors = li.LibraryItem.Resource.ResourceAuthors.Select(ra => ra.Author.Name).ToList(),
                    PrimaryObjectKey = li.LibraryItem.Resource.ResourceImages
                        .OrderByDescending(ri => ri.IsPrimary).ThenBy(ri => ri.DisplayOrder)
                        .Select(ri => ri.File.ObjectKey).FirstOrDefault(),
                    RequestCode = li.Loan.BorrowRequest != null ? li.Loan.BorrowRequest.RequestCode : "",
                    RequestType = li.Loan.BorrowRequest != null ? (BorrowRequestType?)li.Loan.BorrowRequest.RequestType : null,
                    RequestCreatedAt = li.Loan.BorrowRequest != null ? (DateTime?)li.Loan.BorrowRequest.CreatedAt : null
                })
                .ToListAsync(cancellationToken);

            foreach (var x in borrowedList)
            {
                int daysLeft = (int)Math.Ceiling((x.DueAt - now).TotalDays);
                string status = (x.ItemLoanStatus == LoanItemStatus.OVERDUE || daysLeft < 0) ? "overdue" : (daysLeft <= 3 ? "due_soon" : "active");
                string coverUrl = !string.IsNullOrEmpty(x.PrimaryObjectKey)
                    ? await _fileStorageService.CreatePublicGetUrlAsync(x.PrimaryObjectKey)
                    : "/images/default-cover.jpg";

                allItems.Add(new BorrowItemListResponse
                {
                    BorrowItemId = x.BorrowRequestItemId,
                    LoanItemId = x.LoanItemId,
                    ReturnedAt = x.ReturnedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",
                    BookId = x.Resource?.Id ?? Guid.Empty,
                    BookTitle = x.Resource?.Title ?? "Không rõ",
                    Author = x.Authors.Any() ? string.Join(", ", x.Authors) : "Chưa rõ tác giả",
                    CoverUrl = coverUrl,
                    BorrowItemStatus = status,
                    DaysLeft = daysLeft,
                    BorrowDate = x.BorrowedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    DueDate = x.DueAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    RenewCount = x.RenewCount,
                    CanCancel = false,
                    RequestCode = x.RequestCode,
                    Method = x.RequestType == BorrowRequestType.DELIVERY ? "Giao tận nơi" : "Nhận tại thư viện",
                    RequestDate = x.RequestCreatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? ""
                });
            }

            var activeRequestStatuses = new[] {
            BorrowRequestStatus.PENDING, BorrowRequestStatus.APPROVED, BorrowRequestStatus.READY_FOR_PICKUP, BorrowRequestStatus.DELIVERING
        };
            var activeItems = await FetchRequestItemsMappedAsync(memberId, activeRequestStatuses, cancellationToken, isHistoryTab: false);
            allItems.AddRange(activeItems);

            allItems = allItems
                .OrderBy(i => i.BorrowItemStatus == "overdue" ? 0 : (i.BorrowItemStatus == "due_soon" ? 1 : (i.BorrowItemStatus == "active" ? 2 : 3)))
                .ThenByDescending(i => i.BorrowDate ?? i.RequestDate)
                .ToList();
        }
        else if (cleanType == "history")
        {

            var historyRequestStatuses = new[] {
            BorrowRequestStatus.COMPLETED, BorrowRequestStatus.CANCELLED, BorrowRequestStatus.EXPIRED, BorrowRequestStatus.REJECTED
        };
            var historyItems = await FetchRequestItemsMappedAsync(memberId, historyRequestStatuses, cancellationToken, isHistoryTab: true);
            allItems.AddRange(historyItems);

            var finalizedLoanStatuses = new[] { LoanItemStatus.RETURNED, LoanItemStatus.LOST };
            var pastLoans = await _context.LoanItems
                .Where(li => li.Loan.MemberId == memberId && finalizedLoanStatuses.Contains(li.Status))
                .Select(li => new
                {
                    LoanItemId = li.Id,
                    ReturnedAt = li.ReturnedAt,
                    ItemLoanStatus = li.Status,
                    BorrowRequestItemId = _context.BorrowRequestItems
                        .Where(bri => bri.BorrowRequestId == li.Loan.BorrowRequestId && bri.ResourceId == li.LibraryItem.ResourceId)
                        .Select(bri => (Guid?)bri.Id)
                        .FirstOrDefault() ?? Guid.Empty,
                    li.DueAt,
                    BorrowedAt = li.Loan.BorrowedAt,
                    Resource = li.LibraryItem.Resource,
                    Authors = li.LibraryItem.Resource.ResourceAuthors.Select(ra => ra.Author.Name).ToList(),
                    PrimaryObjectKey = li.LibraryItem.Resource.ResourceImages
                        .OrderByDescending(ri => ri.IsPrimary).ThenBy(ri => ri.DisplayOrder)
                        .Select(ri => ri.File.ObjectKey).FirstOrDefault(),
                    RequestCode = li.Loan.BorrowRequest != null ? li.Loan.BorrowRequest.RequestCode : "",
                    RequestType = li.Loan.BorrowRequest != null ? (BorrowRequestType?)li.Loan.BorrowRequest.RequestType : null,
                    RequestCreatedAt = li.Loan.BorrowRequest != null ? (DateTime?)li.Loan.BorrowRequest.CreatedAt : null
                })
                .ToListAsync(cancellationToken);

            foreach (var x in pastLoans)
            {
                string coverUrl = !string.IsNullOrEmpty(x.PrimaryObjectKey)
                    ? await _fileStorageService.CreatePublicGetUrlAsync(x.PrimaryObjectKey)
                    : "/images/default-cover.jpg";

                allItems.Add(new BorrowItemListResponse
                {
                    BorrowItemId = x.BorrowRequestItemId,
                    LoanItemId = x.LoanItemId,
                    ReturnedAt = x.ReturnedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",
                    BookId = x.Resource?.Id ?? Guid.Empty,
                    BookTitle = x.Resource?.Title ?? "Không rõ",
                    Author = x.Authors.Any() ? string.Join(", ", x.Authors) : "Chưa rõ tác giả",
                    CoverUrl = coverUrl,
                    BorrowItemStatus = x.ItemLoanStatus.ToString().ToLower(),
                    BorrowDate = x.BorrowedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    DueDate = x.DueAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    CanCancel = false,
                    RequestCode = x.RequestCode,
                    Method = x.RequestType == BorrowRequestType.DELIVERY ? "Giao tận nơi" : "Nhận tại thư viện",
                    RequestDate = x.RequestCreatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? ""
                });
            }

            allItems = allItems.OrderByDescending(i => i.ReturnedAt ?? i.RequestDate).ToList();
        }

        int totalItems = allItems.Count;
        var pagedItems = allItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<BorrowItemListResponse>
        {
            Items = pagedItems,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }



    private async Task<List<BorrowItemListResponse>> FetchRequestItemsMappedAsync(Guid memberId, BorrowRequestStatus[] requestStatuses, CancellationToken cancellationToken, bool isHistoryTab)
    {
        var query = _context.BorrowRequestItems
            .Where(bri => bri.BorrowRequest.MemberId == memberId);

        if (isHistoryTab)
        {
            query = query.Where(bri => requestStatuses.Contains(bri.BorrowRequest.Status) ||
                                       bri.Status == BorrowRequestItemStatus.CANCELLED ||
                                       bri.Status == BorrowRequestItemStatus.REJECTED ||
                                       bri.Status == BorrowRequestItemStatus.COMPLETED); // Thêm trạng thái hoàn thành vào history
        }
        else
        {
            // 🌟 ĐIỂM CHỐT SỬA LỖI TRÙNG LẶP: 
            // Ở tab Đang xử lý, CHỈ LẤY những item CHƯA biến thành LoanItem (LibraryItemId == null hoặc Status != COMPLETED)
            query = query.Where(bri => requestStatuses.Contains(bri.BorrowRequest.Status) &&
                                       bri.Status != BorrowRequestItemStatus.CANCELLED &&
                                       bri.Status != BorrowRequestItemStatus.REJECTED &&
                                       bri.Status != BorrowRequestItemStatus.COMPLETED);
        }

        var requestItems = await query
            .OrderByDescending(bri => bri.BorrowRequest.CreatedAt)
            .Select(bri => new
            {
                BorrowRequestItemId = bri.Id,
                ItemStatus = bri.Status,
                BorrowRequestStatus = bri.BorrowRequest.Status,
                RequestType = bri.BorrowRequest.RequestType,
                CreatedAt = bri.BorrowRequest.CreatedAt,
                RequestCode = bri.BorrowRequest.RequestCode,
                Resource = bri.Resource,
                RawReturnedAt = _context.LoanItems
                    .Where(li => li.Loan.BorrowRequestId == bri.BorrowRequestId && li.LibraryItem.ResourceId == bri.ResourceId)
                    .Select(li => li.ReturnedAt)
                    .FirstOrDefault(),
                MatchedLoanItemId = _context.LoanItems
                    .Where(li => li.Loan.BorrowRequestId == bri.BorrowRequestId && li.LibraryItem.ResourceId == bri.ResourceId)
                    .Select(li => (Guid?)li.Id)
                    .FirstOrDefault() ?? Guid.Empty,
                Authors = bri.Resource.ResourceAuthors.Select(ra => ra.Author.Name).ToList(),
                PrimaryObjectKey = bri.Resource.ResourceImages
                    .OrderByDescending(ri => ri.IsPrimary).ThenBy(ri => ri.DisplayOrder)
                    .Select(ri => ri.File.ObjectKey).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var invalidReservationStatuses = new[] {
        BorrowRequestStatus.COMPLETED,
        BorrowRequestStatus.REJECTED,
        BorrowRequestStatus.CANCELLED,
        BorrowRequestStatus.EXPIRED
    };

        var tasks = requestItems.Select(async x =>
        {
            string coverUrl = !string.IsNullOrEmpty(x.PrimaryObjectKey)
                ? await _fileStorageService.CreatePublicGetUrlAsync(x.PrimaryObjectKey)
                : "/images/default-cover.jpg";

            bool isItemFinalized = x.ItemStatus == BorrowRequestItemStatus.CANCELLED ||
                                   x.ItemStatus == BorrowRequestItemStatus.COMPLETED ||
                                   x.ItemStatus == BorrowRequestItemStatus.REJECTED;

            bool isAllowedByDelivery = x.RequestType == BorrowRequestType.DELIVERY && x.BorrowRequestStatus != BorrowRequestStatus.DELIVERING;
            bool isAllowedByReservation = x.RequestType == BorrowRequestType.RESERVATION && !invalidReservationStatuses.Contains(x.BorrowRequestStatus);

            bool canCancelComputed = !isItemFinalized && (isAllowedByDelivery || isAllowedByReservation);

            return new BorrowItemListResponse
            {
                BorrowItemId = x.BorrowRequestItemId,
                LoanItemId = x.MatchedLoanItemId,
                BookId = x.Resource?.Id ?? Guid.Empty,
                BookTitle = x.Resource?.Title ?? "Không rõ",
                Author = x.Authors.Any() ? string.Join(", ", x.Authors) : "Chưa rõ tác giả",
                CoverUrl = coverUrl,
                ReturnedAt = x.RawReturnedAt?.ToString("dd/MM/yyyy") ?? "",
                BorrowItemStatus = x.ItemStatus.ToString().ToLower(),
                BorrowRequestStatus = x.BorrowRequestStatus.ToString().ToLower(),
                RequestDate = x.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Method = x.RequestType == BorrowRequestType.DELIVERY ? "Giao tận nơi" : "Nhận tại thư viện",
                RequestCode = x.RequestCode,
                CanCancel = canCancelComputed
            };
        });

        return (await Task.WhenAll(tasks)).ToList();
    }



}