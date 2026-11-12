namespace WebsiteQuanLyThuVien.ViewModels
{
    public class BorrowManagementViewModel
    {
        public List<BorrowItemViewModel> BorrowedItems { get; set; } = new();
        public int RequestCount { get; set; }
    }

    public class BorrowItemViewModel
    {
        public Guid Id { get; set; } // loan_item_id
        public Guid BookId { get; set; } // resource_id
        public string BookTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // active, due_soon, overdue
        public int DaysLeft { get; set; }
        public string BorrowDate { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public int RenewCount { get; set; }
    }


}