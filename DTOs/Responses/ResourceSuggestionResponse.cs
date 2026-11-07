namespace WebsiteQuanLyThuVien.DTOs.Responses;

public class ResourceSuggestionResponse
{
    public List<ResourceSuggestionItem> Items { get; set; } = new List<ResourceSuggestionItem>();
    public class ResourceSuggestionItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? CoverUrl { get; set; }

        public List<string> Categories { get; set; } = new();

        public int AvailableCopies { get; set; }
        public List<string> Authors { get; set; } = new();
        public int PublishYear { get; set; }
    }
}