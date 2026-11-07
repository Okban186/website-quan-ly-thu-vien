using System.IdentityModel.Tokens.Jwt;
using WebsiteQuanLyThuVien.DTOs.Responses;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Extensions;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.Services.Storage;

namespace WebsiteQuanLyThuVien.Services.Uploads;

public class UploadSessionService : IUploadSessionService
{
    private readonly IUploadSessionRepository _uploadSessionRepository;
    private readonly ICardRegistrationRepository _cardRegistrationRepository;
    private readonly IFileStorageService _fileStorageService;

    private readonly IStorageFileRepository _storageFileRepository;

    public UploadSessionService(IUploadSessionRepository uploadSessionRepository, ICardRegistrationRepository cardRegistrationRepository, IFileStorageService fileStorageService, IStorageFileRepository storageFileRepository)
    {
        _uploadSessionRepository = uploadSessionRepository;
        _cardRegistrationRepository = cardRegistrationRepository;
        _fileStorageService = fileStorageService;
        _storageFileRepository = storageFileRepository;
    }

    public Task CancelAsync(Guid uploadSessionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> CompleteAsync(Guid uploadSessionId, CancellationToken cancellationToken = default)
    {
        // Lấy session
        var session = await _uploadSessionRepository.GetByIdAsync(uploadSessionId, cancellationToken);

        if (session is null)
        {
            throw new BusinessException("Không tìm thấy upload.");
        }

        // Xử lý IDEMPOTENCY: Nếu session đã COMPLETED từ trước, trả về luôn StorageFile tương ứng
        if (session.Status == UploadSessionStatus.COMPLETED)
        {
            // Tìm storageFile dựa vào final ObjectKey hiện tại của session
            var existingFile = await _storageFileRepository.GetByObjectKeyAsync(session.ObjectKey, cancellationToken);
            if (existingFile != null)
            {
                return existingFile.Id;
            }
            // Trường hợp hi hữu đã completed nhưng mất record file thì xử lý tạo mới hoặc báo lỗi
        }

        // Kiểm tra trạng thái (cho phép PENDING hoặc FAILED nếu muốn cho retry, nhưng chuẩn nhất vẫn là PENDING)
        if (session.Status != UploadSessionStatus.PENDING && session.Status != UploadSessionStatus.FAILED)
        {
            throw new BusinessException("Upload không ở trạng thái hợp lệ để hoàn thành.");
        }

        // Kiểm tra hết hạn
        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            session.Status = UploadSessionStatus.EXPIRED;
            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);
            throw new BusinessException("Upload đã hết hạn.");
        }

        // Kiểm tra ở temp (session.ObjectKey) HOẶC kiểm tra xem đã từng move sang final prefix chưa
        bool fileExistsInTemp = await _fileStorageService.ExistsAsync(session.ObjectKey, cancellationToken);

        string targetObjectKey = session.ObjectKey;

        if (!fileExistsInTemp)
        {
            // Phòng hờ trường hợp lần trước đã move sang thư mục chính thức nhưng chưa kịp update session DB
            // Ta có thể check xem session có đang giữ cấu trúc prefix chính thức hay không, hoặc search storage xem file đã tồn tại chưa.
            // Hoặc đơn giản: Nếu file không ở temp, ta báo lỗi không tìm thấy file.

            session.Status = UploadSessionStatus.FAILED;
            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);
            throw new BusinessException("Không tìm thấy file đã upload hoặc file đã bị xử lý trước đó.");
        }

        // Lấy thông tin file
        var objectInfo = await _fileStorageService.GetObjectInfoAsync(session.ObjectKey, cancellationToken);

        // Kiểm tra kích thước
        var maxFileSize = session.UploadType.GetMaxFileSize();
        if (objectInfo.Size > maxFileSize)
        {
            session.Status = UploadSessionStatus.FAILED;
            await _fileStorageService.DeleteAsync(session.ObjectKey, cancellationToken);
            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);
            throw new BusinessException("File vượt quá kích thước cho phép.");
        }

        // Detect MIME thật
        var actualMimeType = await _fileStorageService.DetectRealContentTypeAsync(session.ObjectKey, cancellationToken);

        // Kiểm tra MIME theo UploadType
        var allowedMimeTypes = session.UploadType.GetAllowedMimeTypes();
        if (!allowedMimeTypes.Contains(actualMimeType))
        {
            session.Status = UploadSessionStatus.FAILED;
            await _fileStorageService.DeleteAsync(session.ObjectKey, cancellationToken);
            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);
            throw new BusinessException($"Loại file không được phép: {actualMimeType}");
        }

        string extension = UploadTypeExtensions.GetExtensionFromMimeType(actualMimeType);

        // Tạo key chính thức
        var finalObjectKey = $"{session.UploadType.GetFinalPrefix()}/{Guid.NewGuid()}{extension}";

        // Move từ temp sang vị trí chính thức
        await _fileStorageService.MoveAsync(session.ObjectKey, finalObjectKey, cancellationToken);

        // Tạo bản ghi StorageFile trước
        var storageFile = new StorageFile
        {
            Id = Guid.NewGuid(),
            ObjectKey = finalObjectKey,
            ContentType = actualMimeType,
            FileSize = objectInfo.Size,
            CreatedAt = DateTime.UtcNow
        };

        await _storageFileRepository.CreateAsync(storageFile);

        // Cập nhật session sau cùng
        session.ObjectKey = finalObjectKey;
        session.Status = UploadSessionStatus.COMPLETED;
        session.CompletedAt = DateTime.UtcNow;

        await _uploadSessionRepository.UpdateAsync(session, cancellationToken);

        return storageFile.Id;
    }

    public async Task<CreateUploadSessionResponse> CreateAsync(UploadType uploadType, Guid? userId, Guid? cardRegistrationId = null, CancellationToken cancellationToken = default)
    {
        // Nếu có CardRegistrationId thì kiểm tra registration
        if (cardRegistrationId.HasValue)
        {
            var registration = await _cardRegistrationRepository.GetByIdReadOnlyAsync(cardRegistrationId.Value, cancellationToken);

            if (registration is null)
            {
                throw new BusinessException("Không tìm thấy đăng ký thẻ thư viện.");
            }

            if (registration.Status != CardRegistrationStatus.PENDING)
            {
                throw new BusinessException("Đăng ký thẻ không còn ở trạng thái chờ xử lý.");
            }
        }

        // Lấy giới hạn file từ UploadType
        var maxFileSize = uploadType.GetMaxFileSize();

        // Tạo UploadSessionId
        var uploadSessionId = Guid.NewGuid();

        // Tạo temporary object key
        var objectKey = $"temp/uploads/{uploadSessionId}";

        // Thời gian hết hạn của upload session
        var expiration = TimeSpan.FromMinutes(2);

        var expiresAt = DateTime.UtcNow.Add(expiration);


        // Tạo Presigned POST
        var presignedPost = await _fileStorageService.CreatePresignedPostAsync(
                objectKey,
                null,
                maxFileSize,
                expiration,
                cancellationToken);


        // Tạo UploadSession
        var session = new UploadSession
        {
            Id = uploadSessionId,

            UserId = userId,

            CardRegistrationId = cardRegistrationId,

            UploadType = uploadType,

            ObjectKey = objectKey,

            Status = UploadSessionStatus.PENDING,

            ExpiresAt = expiresAt,

            CreatedAt = DateTime.UtcNow
        };

        // Lưu DB
        await _uploadSessionRepository.CreateAsync(session, cancellationToken);

        // 10. Trả thông tin cho client
        return new CreateUploadSessionResponse
        {
            UploadSessionId = uploadSessionId,
            UploadUrl = presignedPost.Url,
            Fields = presignedPost.Fields,
            ExpiresAt = session.ExpiresAt
        };
    }





    public async Task<BatchUploadResultResponse> CompleteBatchAsync(IEnumerable<Guid> uploadSessionIds, CancellationToken cancellationToken = default)
    {
        var response = new BatchUploadResultResponse();

        var tasks = uploadSessionIds.Select(async id =>
        {
            try
            {
                var storageFileId = await CompleteAsync(id, cancellationToken);

                // Thành công trả về HTTP 200 OK
                return new BatchItemResult(id, true, StatusCodes.Status200OK, null, storageFileId);
            }
            catch (BusinessException ex)
            {

                return new BatchItemResult(id, false, ex.StatusCode, ex.Message, null);
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống bất ngờ -> Trả về HTTP 500 Internal Server Error
                return new BatchItemResult(id, false, StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi hệ thống nội bộ.", null);
            }
        });

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            if (result.IsSuccess)
            {
                response.Successful.Add(result);
            }
            else
            {
                response.Failed.Add(result);
            }
        }

        return response;
    }


}