using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.ViewModels;

namespace WebsiteQuanLyThuVien.Services;

public interface IAccountService
{
    Task<UserAccountViewModel> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ChangePasswordAsync(string userIdStr, ChangePasswordRequest request, CancellationToken cancellationToken = default);


    Task UpdateContactAsync(string userIdStr, UpdateUserContactRequest request, CancellationToken cancellationToken = default);
    Task ReportLostCardAsync(string userIdStr, CancellationToken cancellationToken = default);
}