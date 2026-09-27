using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public interface IStorageFileRepository
{
    Task CreateAsync(StorageFile storageFile, CancellationToken cancellationToken = default);
}