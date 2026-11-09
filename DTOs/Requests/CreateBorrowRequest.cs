using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyThuVien.DTOs.Requests;

public class CreateBorrowRequestDto
{
    public List<Guid> ResourceIds { get; set; } = new();

    public string RequestType { get; set; } = "RESERVATION";

    public string? DeliveryAddress { get; set; }
    public string? RecipientPhone { get; set; }

    public string? Note { get; set; }
}