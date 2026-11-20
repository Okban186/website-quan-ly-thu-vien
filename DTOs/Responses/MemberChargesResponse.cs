namespace WebsiteQuanLyThuVien.DTOs.Responses;

public class MemberChargesResponse
{


    public ChargeSummaryDto Summary { get; set; } = new();

    public PagedResult<ChargeListItemDto> Charges { get; set; } = new();
}