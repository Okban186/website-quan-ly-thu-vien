
namespace WebsiteQuanLyThuVien.Models;

public class ChargeType
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


    public ICollection<Charge> Charges { get; set; } = new List<Charge>();

}
