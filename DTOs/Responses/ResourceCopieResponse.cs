using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.Enums;

public class ResourceCopiesResponse
{
    public int AvailableCopies { get; set; }
    public int BorrowingCopies { get; set; }
    public int ProcessingCopies { get; set; }

    public PagedResult<ResourceCopyItem> Copies { get; set; } = new();

    public class ResourceCopyItem
    {
        public string Barcode { get; set; } = null!;
        public LibraryItemStatus Status { get; set; }
        public string? LocationName { get; set; }
    }
}