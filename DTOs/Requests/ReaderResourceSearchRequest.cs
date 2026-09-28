namespace WebsiteQuanLyThuVien.DTOs.Requests;

public class ReaderResourceSearchRequest
{
    public string? Keyword { get; set; }

    public string? FilterExpression { get; set; }

    public int? PublicationYear { get; set; }

    public string Status { get; set; } = "all";

    public string Sort { get; set; } = "relevance";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 12;
}