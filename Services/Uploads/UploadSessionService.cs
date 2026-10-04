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
        //Lấy session
        var session = await _uploadSessionRepository.GetByIdAsync(uploadSessionId, cancellationToken);

        if (session is null)
        {
            throw new BusinessException("Không tìm thấy upload.");
        }

        //Kiểm tra trạng thái
        if (session.Status != UploadSessionStatus.PENDING)
        {
            throw new BusinessException("Upload không ở trạng thái chờ xử lý.");
        }

        //Kiểm tra hết hạn
        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            session.Status = UploadSessionStatus.EXPIRED;

            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);

            throw new BusinessException("Upload đã hết hạn.");
        }

        //Kiểm tra file tồn tại
        var exists = await _fileStorageService.ExistsAsync(session.ObjectKey, cancellationToken);

        if (!exists)
        {
            session.Status = UploadSessionStatus.FAILED;

            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);

            throw new BusinessException("Không tìm thấy file đã upload.");
        }

        //Lấy thông tin file
        var objectInfo = await _fileStorageService.GetObjectInfoAsync(session.ObjectKey, cancellationToken);

        //Kiểm tra kích thước
        var maxFileSize = session.UploadType.GetMaxFileSize();

        if (objectInfo.Size > maxFileSize)
        {
            session.Status = UploadSessionStatus.FAILED;

            await _fileStorageService.DeleteAsync(session.ObjectKey, cancellationToken);

            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);

            throw new BusinessException("File vượt quá kích thước cho phép.");
        }

        //Detect MIME thật
        var actualMimeType = await _fileStorageService.DetectRealContentTypeAsync(session.ObjectKey, cancellationToken);

        //Kiểm tra MIME theo UploadType
        var allowedMimeTypes = session.UploadType.GetAllowedMimeTypes();

        if (!allowedMimeTypes.Contains(actualMimeType))
        {
            session.Status = UploadSessionStatus.FAILED;

            await _fileStorageService.DeleteAsync(session.ObjectKey, cancellationToken);

            await _uploadSessionRepository.UpdateAsync(session, cancellationToken);
            throw new BusinessException($"Loại file không được phép: {actualMimeType}");
        }

        string extension = UploadTypeExtensions.GetExtensionFromMimeType(actualMimeType);
        //Tạo key chính thức
        var finalObjectKey = $"{session.UploadType.GetFinalPrefix()}/{Guid.NewGuid()}{extension}";

        //Move từ temp sang vị trí chính thức
        await _fileStorageService.MoveAsync(session.ObjectKey, finalObjectKey, cancellationToken);

        //Cập nhật session
        session.ObjectKey = finalObjectKey;
        session.Status = UploadSessionStatus.COMPLETED;
        session.CompletedAt = DateTime.UtcNow;

        var storageFile = new StorageFile
        {
            Id = Guid.NewGuid(),
            ObjectKey = finalObjectKey,
            ContentType = actualMimeType,
            FileSize = objectInfo.Size,
            CreatedAt = DateTime.UtcNow
        };

        await _storageFileRepository.CreateAsync(storageFile);

        //Lưu DB
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


}