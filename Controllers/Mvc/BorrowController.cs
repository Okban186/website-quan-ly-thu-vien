using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebsiteQuanLyThuVien.Data;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Enums;
using WebsiteQuanLyThuVien.ViewModels;
using WebsiteQuanLyThuVien.Services.Storage;

[AuthorizeExceptRole("ADMIN")]
[Route("borrows")]
public class BorrowController : Controller
{
    private readonly ApplicationDbContext _context;

    private readonly IFileStorageService _fileStorageService;

    public BorrowController(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    [Route("management")]
    public async Task<IActionResult> Index()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Challenge();

        var member = await _context.LibraryMembers.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null) return View(new BorrowManagementViewModel());

        Guid memberId = member.Id;
        DateTime now = DateTime.UtcNow;





        var requests = await _context.BorrowRequests
            .Where(br => br.MemberId == memberId)
            .OrderByDescending(br => br.CreatedAt)
            .Select(br => new
            {
                br.Id,
                br.Status,
                br.CreatedAt,
                br.RequestType,
                br.RequestCode,
                FirstResource = br.Items.Select(bri => bri.Resource).FirstOrDefault(),
                Authors = br.Items.Select(bri => bri.Resource)
                    .SelectMany(r => r.ResourceAuthors)
                    .Select(ra => ra.Author.Name)
                    .ToList(),
                PrimaryObjectKey = br.Items.Select(bri => bri.Resource)
                    .SelectMany(r => r.ResourceImages)
                    .OrderByDescending(ri => ri.IsPrimary)
                    .ThenBy(ri => ri.DisplayOrder)
                    .Select(ri => ri.File.ObjectKey)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken: default);



        int requestCount = await _context.BorrowRequests
            .CountAsync(br => br.MemberId == memberId, cancellationToken: default);




        var viewModel = new BorrowManagementViewModel
        {
            RequestCount = requestCount
        };

        return View(viewModel);
    }
}