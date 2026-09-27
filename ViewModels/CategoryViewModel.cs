namespace WebsiteQuanLyThuVien.ViewModels;

public class CategoryViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description {get;set;} = string.Empty;

    public int BookCount {get;set;} = 0;
}

