namespace WebsiteQuanLyThuVien.DTOs.Responses;

public class BorrowItemListResponse
{

    public Guid BorrowItemId { get; set; }
    public Guid LoanItemId { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string CoverUrl { get; set; } = string.Empty;
    public string BorrowItemStatus { get; set; } = string.Empty;

    public bool CanCancel { get; set; } = false;

    public string BorrowRequestStatus { get; set; } = string.Empty;


    public int? DaysLeft { get; set; }
    public string BorrowDate { get; set; } = string.Empty;
    public string DueDate { get; set; } = string.Empty;
    public int? RenewCount { get; set; }

    public string ReturnedAt { get; set; } = string.Empty;

    public string RequestDate { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string RequestCode { get; set; } = string.Empty;

    public string CarrierTrackingCode { get; set; } = "";

}