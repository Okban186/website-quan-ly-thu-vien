namespace WebsiteQuanLyThuVien.DTOs.Responses;

public sealed class CreateUploadSessionResponse
{
    /// <summary>
    /// Id của phiên upload, sẽ được sử dụng để xác thực khi hoàn tất hoặc hủy bỏ phiên upload.
    /// </summary>
    public Guid UploadSessionId { get; init; }

    /// <summary>
    /// URL có chữ ký số (Presigned URL) được sinh ra từ MinIO, cho phép Client (Frontend) có thể upload trực tiếp tệp tin lên hệ thống lưu trữ mà không cần đi qua Web Server.
    /// </summary>
    public string UploadUrl { get; init; } = null!;


    /// <summary>
    /// Do sử dụng post cho upload nên MinIO sẽ trả về một số trường dữ liệu (Fields) cần thiết để gửi kèm theo file khi thực hiện upload.
    /// Các trường này sẽ được trả về cho Client (Frontend) để Client có thể gửi trực tiếp file lên MinIO mà không cần đi qua Web Server.
    /// </summary>
    public IDictionary<string, string> Fields { get; init; }

    /// <summary>
    /// Thời điểm hết hạn của URL có chữ ký số (Presigned URL) và các trường dữ liệu (Fields) đi kèm. Sau thời điểm này, Client sẽ không thể upload file lên MinIO bằng URL và Fields này nữa.
    /// </summary>
    public DateTime ExpiresAt { get; init; }
}