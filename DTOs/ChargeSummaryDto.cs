namespace WebsiteQuanLyThuVien.DTOs;

public class ChargeSummaryDto
{
    public decimal TotalUnpaid { get; set; }
    public int UnpaidCount { get; set; }
    public decimal TotalPaid { get; set; }
    public int PaidCount { get; set; }

    public PagedResult<ChargeListItemDto> Charges { get; set; } = new();
}