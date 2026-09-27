using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Repositories;

namespace WebsiteQuanLyThuVien.Services;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumenTypeRepository _documentTypeRepository;

    public DocumentTypeService(IDocumenTypeRepository documenTypeRepository)
    {
        _documentTypeRepository = documenTypeRepository;
    }

    public async Task<List<LookupResponse>> SearchAsync(string? name, int limit, CancellationToken cancellationToken = default)
    {
        return await _documentTypeRepository.SearchAsync(name, limit);
    }
}