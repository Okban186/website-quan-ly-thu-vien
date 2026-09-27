using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

public class Book
{
    public Guid Id { get; set; }

    public string? Isbn { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? PublicationYear { get; set; }

    public string? Language { get; set; }

    public int? PageCount { get; set; }

    public decimal? Price { get; set; }

    public Guid? PublisherId { get; set; }

    public Guid? DocumentTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Publisher? Publisher { get; set; }

    public DocumentType? DocumentType { get; set; }

    public BookStatus Status { get; set; } = BookStatus.ACTIVE;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();


    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}
