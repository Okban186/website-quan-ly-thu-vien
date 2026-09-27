namespace WebsiteQuanLyThuVien.DTOs.Response;
public class BookSearchResponse
{
    public IReadOnlyList<BookSearchItemDto> Items { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}