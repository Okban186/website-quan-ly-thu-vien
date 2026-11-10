
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.ViewModels;

namespace WebsiteQuanLyThuVien.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserAccountViewModel> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rawData = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                User = u,
                Member = _context.LibraryMembers.FirstOrDefault(m => m.UserId == u.Id),
                IsStaffOrAdmin = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .Any(roleName => roleName != "MEMBER")
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (rawData == null) return null;

        if (rawData.Member == null)
        {
            return new UserAccountViewModel
            {
                FullName = rawData.User.FullName,
                Quota = rawData.IsStaffOrAdmin ? 10 : 15,
                Email = rawData.User.Email,
                Phone = rawData.User.Phone,
                BirthDate = rawData.User.DateOfBirth?.ToString("dd/MM/yyyy"),
                IdCard = rawData.User.CitizenId,
                Address = rawData.User.Address,
                CardNumber = "Chưa có thẻ",
                CardStatus = null,
                CardExpiryDate = "",
                TotalBorrowed = 0,
                BorrowedCount = 0,
                OverdueCount = 0,
                UnpaidFines = 0
            };
        }

        Guid memberId = rawData.Member.Id;

        var currentCard = await _context.LibraryCards
            .FirstOrDefaultAsync(c => c.MemberId == memberId, cancellationToken);

        if (currentCard != null && currentCard.ExpiredAt.HasValue && currentCard.ExpiredAt.Value < DateTime.UtcNow && currentCard.Status != LibraryCardStatus.EXPIRED)
        {
            currentCard.Status = LibraryCardStatus.EXPIRED;
            await _context.SaveChangesAsync(cancellationToken);
        }


        var memberStats = await _context.LibraryMembers
            .Where(m => m.Id == memberId)
            .Select(m => new
            {

                Card = _context.LibraryCards
                    .Where(c => c.MemberId == memberId)
                    .Select(c => new { c.CardNumber, c.Status, c.IssuedAt, c.ExpiredAt })
                    .FirstOrDefault(),

                TotalBorrowed = _context.Loans.Count(l => l.MemberId == memberId),

                BorrowedCount = _context.Loans.Count(l => l.MemberId == memberId && l.Status == LoanStatus.ACTIVE),

                OverdueCount = _context.LoanItems.Count(li => _context.Loans.Any(l => l.Id == li.LoanId && l.MemberId == memberId) && li.Status == LoanItemStatus.OVERDUE),

                UnpaidFines = _context.Charges
                    .Where(ch => ch.MemberId == memberId && ch.Status == ChargeStatus.UNPAID)
                    .Sum(ch => (decimal?)ch.Amount) ?? 0
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new UserAccountViewModel
        {
            FullName = rawData.User.FullName,
            Quota = rawData.IsStaffOrAdmin ? 10 : 15,
            Email = rawData.User.Email,
            Phone = rawData.User.Phone,
            BirthDate = rawData.User.DateOfBirth?.ToString("dd/MM/yyyy"),
            IdCard = rawData.User.CitizenId,
            Address = rawData.User.Address,


            CardNumber = memberStats?.Card?.CardNumber ?? "Chưa có thẻ",
            CardIssueDate = memberStats?.Card?.IssuedAt?.ToString("dd/MM/yyyy") ?? "",
            CardStatus = memberStats?.Card?.Status.ToString() ?? "Chưa có thẻ",
            CardExpiryDate = memberStats?.Card?.ExpiredAt?.ToString("dd/MM/yyyy") ?? "",


            TotalBorrowed = memberStats?.TotalBorrowed ?? 0,
            BorrowedCount = memberStats?.BorrowedCount ?? 0,
            OverdueCount = memberStats?.OverdueCount ?? 0,
            UnpaidFines = memberStats?.UnpaidFines ?? 0
        };
    }




    public async Task<bool> ChangePasswordAsync(string userIdStr, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userIdStr, out var userId)) throw new BusinessException("Tài khoản không hợp lệ", StatusCodes.Status403Forbidden);

        if (String.IsNullOrEmpty(request.CurrentPassword)) throw new BusinessException("Vui lòng nhập mật khẩu hiện tại");
        if (String.IsNullOrEmpty(request.ConfirmPassword) || String.IsNullOrEmpty(request.NewPassword)) throw new BusinessException("Vui lòng nhập mật khẩu mới");
        if (request.ConfirmPassword.CompareTo(request.NewPassword) != 0) throw new BusinessException("Mật khẩu mới với mật khẩu xác nhận không khớp");
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null) throw new BusinessException("Tài khoản không hợp lệ", StatusCodes.Status403Forbidden);

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new BusinessException("Mật khẩu hiện tại không chính xác.", StatusCodes.Status400BadRequest);
        }


        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;


        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task UpdateContactAsync(string userIdStr, UpdateUserContactRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            throw new BusinessException("ID người dùng không hợp lệ.", StatusCodes.Status400BadRequest);
        }


        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new BusinessException("Số điện thoại không được để trống.", StatusCodes.Status400BadRequest);

        var phoneRegex = new Regex(@"^(03|05|07|08|09)\d{8}$");

        if (!phoneRegex.IsMatch(request.Phone.Trim()))
        {
            throw new BusinessException("Số điện thoại không hợp lệ (phải gồm 10 chữ số và bắt đầu bằng các đầu số 03, 05, 07, 08, 09).", StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrWhiteSpace(request.Address))
            throw new BusinessException("Địa chỉ không được để trống.", StatusCodes.Status400BadRequest);

        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            throw new BusinessException("Email không hợp lệ hoặc để trống.", StatusCodes.Status400BadRequest);

        var cleanedEmail = request.Email.Trim().ToLower();
        var cleanedPhone = request.Phone.Trim();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new BusinessException("Không tìm thấy thông tin tài khoản.", StatusCodes.Status404NotFound);
        }

        bool isEmailOrPhoneTaken = await _context.Users.AnyAsync(u =>
            u.Id != userId && (u.Email.ToLower() == cleanedEmail || u.Phone == cleanedPhone));

        if (isEmailOrPhoneTaken)
        {
            throw new BusinessException("Email hoặc số điện thoại đã được sử dụng bởi tài khoản khác.", StatusCodes.Status400BadRequest);
        }

        user.Phone = cleanedPhone;
        user.Email = cleanedEmail;
        user.Address = request.Address.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task ReportLostCardAsync(string userIdStr, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            throw new BusinessException("ID người dùng không hợp lệ.", StatusCodes.Status400BadRequest);
        }


        var member = await _context.LibraryMembers
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
        {
            throw new BusinessException("Tài khoản chưa đăng ký thành viên thư viện.", StatusCodes.Status404NotFound);
        }


        var card = await _context.LibraryCards
            .FirstOrDefaultAsync(c => c.MemberId == member.Id);

        if (card == null)
        {
            throw new BusinessException("Không tìm thấy thông tin thẻ thư viện.", StatusCodes.Status404NotFound);
        }


        if (card.Status == LibraryCardStatus.BLOCKED)
        {
            throw new BusinessException("Thẻ của bạn đã bị khóa (báo mất) trước đó rồi.", StatusCodes.Status400BadRequest);
        }

        card.Status = LibraryCardStatus.BLOCKED;
        card.UpdatedAt = DateTime.UtcNow;

        _context.LibraryCards.Update(card);
        await _context.SaveChangesAsync();
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }


}