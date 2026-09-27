namespace WebsiteQuanLyThuVien.Models;

public class DocumentType {
    
     public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; } 
    
    public DateTime CreatedAt { get; set; }  

    public DateTime UpdateAt{get;set;}
    
    public ICollection<Book> Books { get; set; } = new List<Book>(); 
}