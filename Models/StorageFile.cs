namespace WebsiteQuanLyThuVien.Models;

public class StorageFile
{
    public Guid Id { get; set; }
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }
    public string ObjectKey { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public User? AvatarUser { get; set; }

    public ICollection<CardRegistration> DocumentFrontRegistrations { get; set; } = new List<CardRegistration>();

    public ICollection<CardRegistration> DocumentBackRegistrations { get; set; } = new List<CardRegistration>();

    public ICollection<CardRegistration> AvatarRegistrations { get; set; } = new List<CardRegistration>();

    public ICollection<ResourceImage> ResourceImages { get; set; } = new List<ResourceImage>();

    public ICollection<DigitalResource> DigitalResources { get; set; } = new List<DigitalResource>();

}