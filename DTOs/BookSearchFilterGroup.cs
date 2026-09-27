namespace WebsiteQuanLyThuVien.DTOs;

public class BookSearchFilterGroup
{
    public Guid Id { get; set; }

    public string Operator { get; set; } = "OR";
}