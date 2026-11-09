using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.DTOs.Responses;

namespace WebsiteQuanLyThuVien.Services;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;

    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
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
        int maxAllowedBooks = roles.Contains("LIBRARIAN") || roles.Contains("STOREKEEPER") || roles.Contains("CATALOGER") ? 15 : 100;

        var activeLoansCount = await _context.LoanItems
            .Include(li => li.Loan)
            .CountAsync(li => li.Loan.MemberId == memberId && li.Loan.Status == LoanStatus.ACTIVE && li.Status == LoanItemStatus.BORROWED, cancellationToken);

        var activeStatuses = new[] { "PENDING", "APPROVED", "READY_FOR_PICKUP", "DELIVERING" };

        var activeRequestsCount = await _context.BorrowRequestItems
            .Include(bri => bri.BorrowRequest)
            .CountAsync(bri => bri.BorrowRequest.MemberId == memberId
                               && activeStatuses.Contains(bri.BorrowRequest.Status.ToString()), cancellationToken);

        // Kiểm tra tổng số sách đang mượn/đặt hiện tại cộng với số lượng muốn đặt mới có vượt trần không
        if (activeLoansCount + activeRequestsCount + request.ResourceIds.Count > maxAllowedBooks)
        {
            throw new BusinessException($"Vượt quá giới hạn cho phép. Bạn chỉ được mượn tối đa {maxAllowedBooks}.", StatusCodes.Status403Forbidden);
        }

        // Kiểm tra tính hợp lệ của từng ResourceId
        foreach (var resourceId in request.ResourceIds.Distinct())
        {
            var resource = await _context.Resources
                .FirstOrDefaultAsync(r => r.Id == resourceId && r.Status == ResourceStatus.ACTIVE, cancellationToken);

            if (resource == null)
            {
                throw new BusinessException($"Tài liệu với ID {resourceId} không tồn tại hoặc đã ngừng cung cấp.");
            }

            // Kiểm tra xem người dùng này đã có request chờ cho resource này chưa (chặn trùng lặp)
            var hasExistingRequest = await _context.BorrowRequestItems
                .Include(bri => bri.BorrowRequest)
                .AnyAsync(bri => bri.BorrowRequest.MemberId == memberId
                               && bri.ResourceId == resourceId
                               && activeStatuses.Contains(bri.BorrowRequest.Status.ToString()), cancellationToken);

            if (hasExistingRequest)
            {
                throw new BusinessException($"Bạn đang có yêu cầu chờ xử lý đối với tài liệu: {resource.Title}", StatusCodes.Status409Conflict);
            }

            // Chỉ đếm các bản sao vật lý đang ở trạng thái AVAILABLE thực tế trong kho (loại trừ MAINTENANCE, LOST, DAMAGED,...)
            int availablePhysicalCount = await _context.LibraryItems
                .CountAsync(li => li.ResourceId == resourceId
                               && li.Status == LibraryItemStatus.AVAILABLE, cancellationToken);

            // Đếm số lượng request đang treo của hệ thống (chưa được thủ kho gán LibraryItemId cụ thể)
            int pendingRequestsCount = await _context.BorrowRequestItems
                .Include(bri => bri.BorrowRequest)
                .CountAsync(bri => bri.ResourceId == resourceId
                               && bri.LibraryItemId == null
                               && activeStatuses.Contains(bri.BorrowRequest.Status.ToString()), cancellationToken);

            // Hạn mức khả dụng thực tế sau khi trừ đi các request đang chờ duyệt của hệ thống
            int availableQuota = availablePhysicalCount - pendingRequestsCount;

            if (availableQuota <= 0)
            {
                throw new BusinessException($"Tài liệu \"{resource.Title}\" hiện đã hết bản khả dụng (Đang bảo trì, mượn hoặc có người đặt trước).", StatusCodes.Status400BadRequest);
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

        // Thêm toàn bộ danh sách các sách vào borrow_request_items
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
}