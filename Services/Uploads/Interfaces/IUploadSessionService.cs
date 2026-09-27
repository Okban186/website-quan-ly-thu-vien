using WebsiteQuanLyThuVien.Services.Uploads;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Responses;

namespace WebsiteQuanLyThuVien.Services.Uploads;

public interface IUploadSessionService
{

    /// <summary>
    /// Tạo đường link để đẩy ảnh lên
    /// </summary>
    /// <param name="uploadType">Upload type cho biết là các kiểu đuôi file được push vào VD như AVATAR thì chỉ cho jpg, png hoặc wepb và chỉ được push tối đa 2mb</param>
    /// <param name="userId">cho upload session biết ai đã push ảnh lên</param>
    /// <param name="cardRegistrationId">dành cho người mới đăng ký thẻ thư viện</param>
    /// <param name="cancellationToken">Giúp cho các tác vụ bất đồng bộ ngừng ngay khi người dùng đóng request giữa chừng</param>
    /// <returns>Trả về object chứa link và các Fields để js gọi sử dụng</returns>
    Task<CreateUploadSessionResponse> CreateAsync(UploadType uploadType, Guid? userId, Guid? cardRegistrationId = null, CancellationToken cancellationToken = default);


    /// <summary>
    /// Sau khi đã push bằng url gọi từ CreateAsync thì sử dụng để xác thực file coi file có đủ điều kiện không và cho nó vào khu vực được sử dụng
    /// </summary>
    /// <param name="uploadSessionId">Id tới bảng UploadSession nơi ánh xạ tới đường dẫn lưu trữ của tài nguyên</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Không trả về gì</returns>gì
    Task CompleteAsync(Guid uploadSessionId, CancellationToken cancellationToken = default);

    Task CancelAsync(Guid uploadSessionId, CancellationToken cancellationToken = default);
}