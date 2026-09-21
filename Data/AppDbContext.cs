using System;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQuanLyThuVien.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
