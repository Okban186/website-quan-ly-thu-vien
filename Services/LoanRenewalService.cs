using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Services;

public class LoanRenewalService : ILoanRenewalService
{
    private readonly ApplicationDbContext _context;

    public LoanRenewalService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> RenewLoanItemAsync(Guid loanItemId, Guid userId, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;

        var loanItem = await _context.LoanItems
            .Include(li => li.Loan)
            .ThenInclude(l => l.Member)
            .Include(li => li.LibraryItem)
            .FirstOrDefaultAsync(li => li.Id == loanItemId);

        if (loanItem == null || loanItem.Status != LoanItemStatus.BORROWED)
            throw new BusinessException("Tài liệu không tồn tại, đã hoàn trả hoặc đã quá hạn, không thể gia hạn.");

        if (loanItem.Loan.Member.UserId != userId)
        {
            throw new BusinessException("Bạn không có quyền xử lý yêu cầu gia hạn cho tài liệu này.");
        }


        var daysLeft = (int)Math.Ceiling((loanItem.DueAt - nowUtc).TotalDays);
        if (daysLeft < 0 || daysLeft > 3)
        {
            throw new BusinessException($"Yêu cầu thất bại. Bạn chỉ được phép gia hạn khi thời hạn trả còn từ 0 đến 3 ngày (Hiện tại bạn còn {daysLeft} ngày).");
        }

        int renewalCount = await _context.LoanRenewals.CountAsync(lr => lr.LoanItemId == loanItemId);
        if (renewalCount >= 1)
            throw new BusinessException("Tài liệu này đã đạt giới hạn số lần gia hạn tối đa (1 lần).");

        var activeRequestStatuses = new[] { BorrowRequestStatus.PENDING, BorrowRequestStatus.APPROVED, BorrowRequestStatus.READY_FOR_PICKUP };


        var oldDueAt = loanItem.DueAt;
        var newDueAt = oldDueAt.Date.AddDays(7).AddHours(23).AddMinutes(59);

        var renewal = new LoanRenewal
        {
            LoanItemId = loanItemId,
            OldDueAt = oldDueAt,
            NewDueAt = newDueAt,
            RenewedBy = userId,
            CreatedAt = nowUtc
        };
        _context.LoanRenewals.Add(renewal);

        loanItem.DueAt = newDueAt;
        loanItem.UpdatedAt = nowUtc;

        await _context.SaveChangesAsync();
        return true;
    }

}