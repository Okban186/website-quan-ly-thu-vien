namespace WebsiteQuanLyThuVien.Services.Storage;


/// <summary>
/// Định nghĩa các dịch vụ tương tác và quản lý tệp tin trên hệ thống lưu trữ minio
/// </summary>
public interface IFileStorageService
{

    /// <summary>
    /// Kiểm tra và đảm bảo rằng Bucket lưu trữ đã tồn tại trong hệ thống. 
    /// Nếu chưa có, hệ thống sẽ tự động khởi tạo một Bucket mới.
    /// </summary>
    Task EnsureBucketExistsAsync();

    /// <summary>
    /// Khởi tạo một chính sách tải lên và sinh ra URL kèm chữ ký số bảo mật (Presigned POST).
    /// Cho phép Client (Frontend) có thể upload tệp tin trực tiếp lên thẳng hệ thống lưu trữ mà không cần đi qua Web Server.
    /// </summary>
    /// <param name="objectKey">Đường dẫn đích cấu hình để lưu tệp tin (Ví dụ: "temp/avatars/user_1.jpg").</param>
    /// <param name="contentType">Định dạng MIME-type bắt buộc của tệp tin (Ví dụ: "image/jpeg", "application/pdf").</param>
    /// <param name="maxFileSize">Dung lượng tối đa cho phép của tệp tin tính bằng đơn vị Byte.</param>
    /// <param name="expiration">Khoảng thời gian hiệu lực của chữ ký và đường dẫn upload này.</param>
    Task<PresignedPostData> CreatePresignedPostAsync(
        string objectKey,
        string contentType,
        long maxFileSize,
        TimeSpan expiration);

    /// <summary>
    /// Kiểm tra xem một tệp tin có thực sự tồn tại trên hệ thống lưu trữ hay không.
    /// </summary>
    Task<bool> ExistsAsync(string objectKey);

    /// <summary>
    /// Lấy thông tin chi tiết (Metadata) của tệp tin mà không cần tải nội dung tệp tin về.
    /// </summary>
    Task<StorageObjectInfo> GetObjectInfoAsync(string objectKey);

    /// <summary>
    /// Tải tệp tin từ hệ thống lưu trữ về dưới dạng một luồng dữ liệu (Stream).
    /// </summary>
    Task<Stream> DownloadAsync(string objectKey);

    /// <summary>
    /// Xóa hoàn toàn một tệp tin khỏi hệ thống lưu trữ.
    /// </summary>
    /// <param name="objectKey">Đường dẫn của tệp tin cần xóa.</param>
    Task DeleteAsync(string objectKey);

    /// <summary>
    /// Di chuyển hoặc đổi tên một tệp tin từ vị trí cũ sang vị trí mới trong hệ thống lưu trữ (Ví dụ: từ thư mục Temp sang Bucket chính).
    /// </summary>
    /// <param name="sourceKey">Đường dẫn gốc hiện tại của tệp tin.</param>
    /// <param name="destinationKey">Đường dẫn đích mới cần di chuyển tới.</param>
    /// <returns>Một <see cref="Task"/> đại diện cho tiến trình bất đồng bộ.</returns>
    Task MoveAsync(
        string sourceKey,
        string destinationKey);


    /// <summary>
    /// Sử dụng để lấy được đuôi file thật thay vì tin vào đuôi file được đưa vào minio
    /// </summary>
    /// <param name="objectKey">Đường dẫn của tệp tin cần kiểm tra</param>
    /// <returns></returns>
    Task<string> DetectRealContentTypeAsync(string objectKey);
}
