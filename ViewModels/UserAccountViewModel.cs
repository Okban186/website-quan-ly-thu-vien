namespace WebsiteQuanLyThuVien.ViewModels;

public class UserAccountViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string BirthDate { get; set; }
    public string IdCard { get; set; }
    public string Address { get; set; }
    public string CardNumber { get; set; }
    public string CardStatus { get; set; } // "valid" hoặc "expired"
    public string CardIssueDate { get; set; }
    public string CardExpiryDate { get; set; }

    public int Quota { get; set; }

    // Các chỉ số thống kê
    public int TotalBorrowed { get; set; }
    public int BorrowedCount { get; set; }
    public int OverdueCount { get; set; }
    public decimal UnpaidFines { get; set; }
}