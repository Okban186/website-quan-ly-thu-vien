using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public class CardRegistrationRepository : ICardRegistrationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CardRegistrationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(CardRegistration cardRegistration, CancellationToken cancellationToken = default)
    {
        await _dbContext.CardRegistrations.AddAsync(cardRegistration, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

   public async Task<CardRegistration?> GetByIdReadOnlyAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _dbContext.CardRegistrations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
    }
}