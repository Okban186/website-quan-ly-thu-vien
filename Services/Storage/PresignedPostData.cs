namespace WebsiteQuanLyThuVien.Services.Storage;

/// <summary>
/// Chứa thông tin URL và chữ ký số bảo mật dùng để Client (Frontend) 
/// có thể tải tệp tin trực tiếp lên thẳng MinIO bằng phương thức HTTP POST.
/// </summary>
public class PresignedPostData
{

   /// <summary>
    /// Đường dẫn Endpoint của MinIO chấp nhận yêu cầu upload (Ví dụ: http://localhost:9000/bucket-name)
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// Các trường dữ liệu (Form Fields) chứa chính sách bảo mật và chữ ký số đã được ký sẵn từ Server.
    /// Client bắt buộc phải đính kèm đầy đủ các trường này vào FormData trước khi gửi.
    /// </summary>
    public IDictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
}