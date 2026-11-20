
namespace WebsiteQuanLyThuVien.DTOs;

public class ChargeListItemDto
{
    public Guid Id { get; set; }

    public string ChargeTypeCode { get; set; } = string.Empty;

    public string ChargeTypeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ResourceTitle { get; set; }

    public string? CoverUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }
}
