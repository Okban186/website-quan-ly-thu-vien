using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public class StorageFileRepository : IStorageFileRepository
{

    private readonly ApplicationDbContext _context;

    public StorageFileRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CreateAsync(StorageFile storageFile, CancellationToken cancellationToken = default)
    {
        await _context.StorageFiles.AddAsync(storageFile, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}