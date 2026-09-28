using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class Resource
{
    public Guid Id { get; set; }

    public string? Isbn { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public int? PublicationYear { get; set; }
    public string? Language { get; set; }
    public int? PageCount { get; set; }
    public decimal? Price { get; set; }

    public Guid? PublisherId { get; set; }
    public Guid? DocumentTypeId { get; set; }

    public string? PhysicalDescription { get; set; }

    public ResourceStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Publisher? Publisher { get; set; }
    public DocumentType? DocumentType { get; set; }

    public ICollection<LibraryItem> LibraryItems { get; set; } = new List<LibraryItem>();

    public ICollection<ResourceCategory> ResourceCategories { get; set; } = new List<ResourceCategory>();

    public ICollection<ResourceAuthor> ResourceAuthors { get; set; } = new List<ResourceAuthor>();

    public ICollection<ResourceImage> ResourceImages { get; set; } = new List<ResourceImage>();

    public ICollection<DigitalResource> DigitalResources { get; set; } = new List<DigitalResource>();

    public ICollection<BorrowRequestItem> BorrowRequestItems { get; set; } = new List<BorrowRequestItem>();
}