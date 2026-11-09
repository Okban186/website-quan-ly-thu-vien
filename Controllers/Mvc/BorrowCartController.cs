using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;

namespace WebsiteQuanLyThuVien.Controllers.Mvc;


[Authorize]
public class BorrowCartController : Controller
{
    private readonly ApplicationDbContext _context;
    public BorrowCartController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> IndexAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");


        List<dynamic> result = await _context.Users
            .Where(u => u.Id == Guid.Parse(userId))
            .Select(u => new { u.Phone, u.Address })
            .ToListAsync<dynamic>();

        return View(result);
    }
}