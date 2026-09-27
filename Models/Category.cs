using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace WebsiteQuanLyThuVien.Models;

public class Category
{
    [Key]
    [Column("id")]
    public Guid Id {get;set;}

    [Required]
    [MaxLength(150)]
    [Column("name")]
    public string Name{get;set;}

    [Column("description")]
    public string Description{get;set;}

    [Column("display_order")]
    public int DisplayOrder{get;set;}

    [Column("created_at")]
    public DateTime CreatedAt{get;set;}

    [Column("updated_at")]
    public DateTime UpdatedAt {get;set;}

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}