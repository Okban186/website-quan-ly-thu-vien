using System.Runtime.CompilerServices;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Responses;

namespace WebsiteQuanLyThuVien.Services;

public interface IReservationService
{
    Task<BorrowRequestResultResponse> CreateRequestAsync(Guid userId, CreateBorrowRequestDto request, CancellationToken cancellationToken = default);

    Task CancelRequestItem(Guid id, CancellationToken cancellationToken = default);


    Task<PagedResult<BorrowItemListResponse>> GetBorrowItemList(Guid userId, string type, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}