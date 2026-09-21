using Microsoft.Extensions.Options;
using MimeDetective;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace WebsiteQuanLyThuVien.Services.Storage;

/// <summary>
/// Dịch vụ quản lý kho lưu trữ tệp tin bằng MinIO, chịu trách nhiệm xử lý mọi thao tác 
/// liên quan đến file như tạo link upload, kiểm tra, tải, xóa và di chuyển tệp tin.
/// </summary>
public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;
    private readonly IContentInspector _inspector;

    /// <summary>
    /// Hàm khởi tạo, nạp cấu hình kết nối MinIO và bộ quét tệp tin Mime-Detective từ hệ thống.
    /// </summary>
    public MinioFileStorageService(IMinioClient minioClient, IOptions<MinioOptions> options, IContentInspector inspector)
    {
        _minioClient = minioClient;
        _options = options.Value;
        _inspector = inspector;
    }

    /// <summary>
    /// Kiểm tra xem cái thùng chứa (Bucket) cấu hình trong hệ thống đã có trên MinIO chưa. 
    /// Nếu chưa có thì tự động tạo luôn một cái mới để tránh lỗi khi lưu file.
    /// </summary>
    public async Task EnsureBucketExistsAsync()
    {
        var existsArgs = new BucketExistsArgs()
            .WithBucket(_options.BucketName);

        var exists = await _minioClient.BucketExistsAsync(existsArgs);

        if (!exists)
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(_options.BucketName);

            await _minioClient.MakeBucketAsync(makeBucketArgs);
        }
    }

    /// <summary>
    /// Tạo ra một (Presigned URL) chứa đầy đủ chữ ký bảo mật và quy định về file.
    /// Frontend sẽ cầm cục dữ liệu này để tự nộp file thẳng lên MinIO không cần qua Server kiểm tra.
    /// </summary>
    /// <param name="objectKey">Đường dẫn và tên file muốn lưu trên MinIO (Ví dụ: temp/avatar.png).</param>
    /// <param name="contentType">Định dạng file bắt buộc phải nộp (Ví dụ: image/png).</param>
    /// <param name="maxFileSize">Giới hạn dung lượng tối đa của file tính bằng byte.</param>
    /// <param name="expiration">Thời gian sống của cái link này, quá hạn là không upload được nữa.</param>
    /// <returns>Một Object chứa địa chỉ kho và bộ thông số form field đã được ký bảo mật. (Dạng Post)</returns>
    public async Task<PresignedPostData> CreatePresignedPostAsync(string objectKey, string contentType, long maxFileSize, TimeSpan expiration)
    {
        var policy = new PostPolicy();

        policy.SetBucket(_options.BucketName);
        policy.SetKey(objectKey);
        policy.SetContentType(contentType);
        policy.SetContentRange(1, maxFileSize);
        policy.SetExpires(DateTime.UtcNow.Add(expiration));

        var args = new PresignedPostPolicyArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectKey)
            .WithPolicy(policy);

        var result = await _minioClient.PresignedPostPolicyAsync(args);

        return new PresignedPostData
        {
            Url = result.Item1.ToString(),
            Fields = result.Item2
        };
    }

    /// <summary>
    /// Kiểm tra nhanh xem một file có thực sự tồn tại ở đường dẫn cấu hình trên MinIO hay không.
    /// </summary>
    /// <param name="objectKey">Đường dẫn của file cần kiểm tra.</param>
    /// <returns>Trả về true nếu tìm thấy file, trả về false nếu file không tồn tại hoặc lỗi mạng.</returns>
    public async Task<bool> ExistsAsync(string objectKey)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectKey);

            await _minioClient.StatObjectAsync(args);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin nhãn dán (Metadata) của file từ hệ thống quản lý của MinIO 
    /// để xem dung lượng và định dạng khai báo của file mà không cần phải tải toàn bộ file về.
    /// </summary>
    /// <param name="objectKey">Đường dẫn file cần lấy thông tin.</param>
    /// <returns>Một gói chứa thông tin kích thước và định dạng đăng ký của tệp tin.</returns>
    public async Task<StorageObjectInfo> GetObjectInfoAsync(string objectKey)
    {
        var args = new StatObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectKey);

        var result = await _minioClient.StatObjectAsync(args);

        return new StorageObjectInfo
        {
            Size = result.Size,
            ContentType = result.ContentType ?? ""
        };
    }

    /// <summary>
    /// Tải một file từ MinIO về và chuyển nó thành một luồng dữ liệu (Stream) trong bộ nhớ tạm 
    /// để Server ASP.NET Core có thể đọc nội dung hoặc chuyển tiếp cho người dùng tải về.
    /// </summary>
    /// <param name="objectKey">Đường dẫn file muốn tải xuống.</param>
    /// <returns>Một luồng dữ liệu MemoryStream chứa ruột của file thô.</returns>
    public async Task<Stream> DownloadAsync(string objectKey)
    {
        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectKey)
            .WithCallbackStream(stream =>
            {
                stream.CopyToAsync(memoryStream);
            });

        await _minioClient.GetObjectAsync(args);

        memoryStream.Position = 0;

        return memoryStream;
    }

    /// <summary>
    /// Xóa sổ hoàn toàn một file ra khỏi thùng chứa trên MinIO theo đường dẫn chỉ định.
    /// </summary>
    /// <param name="objectKey">Đường dẫn file cần xóa bỏ.</param>
    public async Task DeleteAsync(string objectKey)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(args);
    }

    /// <summary>
    /// Di chuyển một file sang vị trí mới (hoặc đổi tên file) bằng cách sao chép file đó 
    /// sang đường dẫn đích rồi quay lại xóa file gốc ở đường dẫn cũ đi. Thường dùng để dọn file từ Temp sang kho chính.
    /// </summary>
    /// <param name="sourceKey">Đường dẫn gốc hiện tại của file.</param>
    /// <param name="destinationKey">Đường dẫn mới muốn dịch chuyển file tới.</param>
    public async Task MoveAsync(string sourceKey, string destinationKey)
    {
        var copyArgs = new CopyObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(destinationKey)
            .WithCopyObjectSource(
                new CopySourceObjectArgs()
                    .WithBucket(_options.BucketName)
                    .WithObject(sourceKey));

        await _minioClient.CopyObjectAsync(copyArgs);

        await DeleteAsync(sourceKey);
    }

    /// <summary>
    /// Vào trong ruột file (quét cấu trúc các byte nhị phân đầu tiên) bằng thư viện Mime-Detective 
    /// để lấy định dạng thật sự của file. Giúp tránh những file giả dạng (như file .exe đổi đuôi thành .png).
    /// </summary>
    /// <param name="objectKey">Đường dẫn file để kiểm tra định dạng thật.</param>
    /// <returns>Chuỗi định dạng Content-Type (Ví dụ: application/pdf), nếu không đoán được thì trả về dạng nhị phân thô.</returns>
    public async Task<string> DetectRealContentTypeAsync(string objectKey)
    {
        try
        {
            using var fileStream = await DownloadAsync(objectKey);

            if (fileStream == null || fileStream.Length == 0)
            {
                return "application/octet-stream";
            }

            fileStream.Position = 0;

            var results = _inspector.Inspect(fileStream).ByMimeType();

            var highestMatch = results.FirstOrDefault();

            if (highestMatch != null && !string.IsNullOrEmpty(highestMatch.MimeType))
            {
                return highestMatch.MimeType;
            }

            return "application/octet-stream";
        }
        catch
        {
            return "application/octet-stream";
        }
    }
}