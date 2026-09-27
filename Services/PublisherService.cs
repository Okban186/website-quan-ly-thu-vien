using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Repositories;

namespace WebsiteQuanLyThuVien.Services;

public class PublisherService : IPublisherService
{
    private readonly IPublisherRepository _publisherRepository;

    public PublisherService(IPublisherRepository publisherRepository)
    {
        _publisherRepository = publisherRepository;
    }

    public async Task<List<LookupResponse>> SearchAsync(string? name, int limit, CancellationToken cancellationToken = default)
    {
        return await _publisherRepository.SearchAsync(name, limit);
    }
}