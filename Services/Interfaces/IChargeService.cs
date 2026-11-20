using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Responses;
using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Services;

public interface IChargeService
{
    Task<MemberChargesResponse> GetMemberChargesAsync(Guid userId, ChargeStatus status, int page, int pageSize, CancellationToken cancellationToken = default);
}