using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Responses;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Services.Storage;

namespace WebsiteQuanLyThuVien.Services;

public class ChargeService : IChargeService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public ChargeService(
        ApplicationDbContext context,
        IFileStorageService fileStorageService
    )
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<MemberChargesResponse> GetMemberChargesAsync(Guid userId, ChargeStatus status, int page, int pageSize, CancellationToken cancellationToken = default)
    {

        if (status != ChargeStatus.UNPAID &&
            status != ChargeStatus.PAID)
        {
            throw new ArgumentException(
                "Trạng thái khoản phí không hợp lệ.",
                nameof(status)
            );
        }

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var memberCharges = _context.Charges
            .AsNoTracking()
            .Where(charge => charge.Member.UserId == userId);

        var summaryData = await memberCharges
            .GroupBy(charge => charge.MemberId)
            .Select(group => new
            {
                TotalUnpaid = group
                    .Where(charge => charge.Status == ChargeStatus.UNPAID)
                    .Sum(charge => (decimal?)charge.Amount) ?? 0m,

                UnpaidCount = group
                    .Count(charge => charge.Status == ChargeStatus.UNPAID),

                TotalPaid = group
                    .Where(charge => charge.Status == ChargeStatus.PAID)
                    .Sum(charge => (decimal?)charge.Amount) ?? 0m,

                PaidCount = group
                    .Count(charge => charge.Status == ChargeStatus.PAID)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var summary = new ChargeSummaryDto
        {
            TotalUnpaid = summaryData?.TotalUnpaid ?? 0m,
            UnpaidCount = summaryData?.UnpaidCount ?? 0,
            TotalPaid = summaryData?.TotalPaid ?? 0m,
            PaidCount = summaryData?.PaidCount ?? 0,
        };


        var filteredCharges = memberCharges
            .Where(charge => charge.Status == status);


        var totalItems = await filteredCharges
            .CountAsync(cancellationToken);



        var chargeRows = await filteredCharges
            .OrderByDescending(charge => charge.CreatedAt)
            .ThenByDescending(charge => charge.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(charge => new
            {
                charge.Id,

                ChargeTypeCode = charge.ChargeType != null
                    ? charge.ChargeType.Code
                    : string.Empty,

                ChargeTypeName = charge.ChargeType != null
                    ? charge.ChargeType.Name
                    : string.Empty,

                charge.Amount,
                charge.Status,
                charge.Description,
                charge.CreatedAt,


                ResourceTitle = charge.LoanItem != null
                    ? charge.LoanItem.LibraryItem.Resource.Title
                    : null,


                CoverObjectKey = charge.LoanItem != null
                    ? charge.LoanItem.LibraryItem.Resource.ResourceImages
                        .OrderByDescending(image => image.IsPrimary)
                        .ThenBy(image => image.DisplayOrder)
                        .Select(image => image.File.ObjectKey)
                        .FirstOrDefault()
                    : null,


                PaidAt = charge.PaymentItems
                    .Where(paymentItem =>
                        paymentItem.Payment.Status ==
                        PaymentTransactionStatus.PAID
                    )
                    .Select(paymentItem =>
                        (DateTime?)paymentItem.Payment.UpdatedAt
                    )
                    .Max()
            })
            .ToListAsync(cancellationToken);



        var chargeItems = chargeRows
            .Select(row => new ChargeListItemDto
            {
                Id = row.Id,
                ChargeTypeCode = row.ChargeTypeCode,
                ChargeTypeName = row.ChargeTypeName,
                Amount = row.Amount,
                Status = row.Status.ToString(),
                Description = row.Description,
                ResourceTitle = row.ResourceTitle,
                CreatedAt = row.CreatedAt,
                PaidAt = row.PaidAt
            })
            .ToList();


        var imageTasks = chargeRows
            .Select(async (row, index) =>
            {
                if (string.IsNullOrWhiteSpace(row.CoverObjectKey))
                {
                    return;
                }

                chargeItems[index].CoverUrl =
                    await _fileStorageService.CreatePublicGetUrlAsync(
                        row.CoverObjectKey,
                        cancellationToken
                    );
            });

        await Task.WhenAll(imageTasks);



        return new MemberChargesResponse
        {
            Summary = summary,

            Charges = new PagedResult<ChargeListItemDto>
            {
                Items = chargeItems,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            }
        };
    }
}