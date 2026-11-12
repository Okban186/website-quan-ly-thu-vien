namespace WebsiteQuanLyThuVien.Services;

public interface ILoanRenewalService
{
    Task<bool> RenewLoanItemAsync(Guid loanItemId, Guid userId, CancellationToken cancellationToken = default);
}