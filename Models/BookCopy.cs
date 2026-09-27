using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteQuanLyThuVien.Enums;

namespace WebsiteQuanLyThuVien.Models;

[Table("book_copies")]
public class BookCopy
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("location_id")]
    public Guid? LocationId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("barcode")]
    public string Barcode { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public BookCopyStatus Status { get; set; } = BookCopyStatus.AVAILABLE;

    [Required]
    [MaxLength(20)]
    [Column("condition")]
    public BookCopyCondition Condition { get; set; } = BookCopyCondition.GOOD;

    [Column("acquired_at")]
    public DateTime? AcquiredAt { get; set; }

    [Column("acquisition_price")]
    public decimal? AcquisitionPrice { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }


    public Book Book { get; set; } = null!;

}