public class AddToCartDto
{
    public Guid ResourceId { get; set; }
}

public class SyncCartDto
{
    public List<Guid> ResourceIds { get; set; } = new();
}

public class BorrowCartItemResponseDto
{
    public Guid ResourceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImage { get; set; }
    public string Authors { get; set; }
    public DateTime AddedAt { get; set; }

    public bool IsAvailable { get; set; }
    public string UnavailableReason { get; set; }
}