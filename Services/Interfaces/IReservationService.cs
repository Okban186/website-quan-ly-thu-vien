using System.Runtime.CompilerServices;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Responses;

namespace WebsiteQuanLyThuVien.Services;

public interface IReservationService
{
    Task<BorrowRequestResultResponse> CreateRequestAsync(Guid userId, CreateBorrowRequestDto request, CancellationToken cancellationToken = default);
}