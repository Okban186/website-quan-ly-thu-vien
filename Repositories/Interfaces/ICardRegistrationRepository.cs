using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public interface ICardRegistrationRepository
{
    Task CreateAsync(CardRegistration cardRegistration, CancellationToken cancellationToken = default);
    Task<CardRegistration?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default);
}