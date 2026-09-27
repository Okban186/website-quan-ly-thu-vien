using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public interface IUploadSessionRepository
{
    Task CreateAsync(UploadSession session, CancellationToken cancellationToken);
    Task<UploadSession?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken);

    Task<UploadSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(UploadSession session, CancellationToken cancellationToken);
}

