using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public class UploadSessionRepository : IUploadSessionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UploadSessionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(UploadSession uploadSession, CancellationToken cancellationToken = default)
    {
        await _dbContext.UploadSessions.AddAsync(uploadSession, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UploadSession?> GetByIdReadOnlyAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _dbContext.UploadSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
    }

    public async Task<UploadSession?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _dbContext.UploadSessions.FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
    }

    public async Task UpdateAsync(UploadSession uploadSession, CancellationToken cancellationToken = default)
    {
        _dbContext.UploadSessions.Update(uploadSession);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}