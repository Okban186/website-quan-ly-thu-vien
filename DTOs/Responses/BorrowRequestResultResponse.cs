namespace WebsiteQuanLyThuVien.DTOs.Responses;

public class BorrowRequestResultResponse
{
    public Guid Id { get; set; }
    public string RequestCode { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
}