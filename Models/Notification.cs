namespace WebsiteQuanLyThuVien.Models;

public class Notification
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? NotificationType { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
}