using System;
using WebsiteQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQuanLyThuVien.Data;
/// <summary>
/// EF core
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<StorageFile> StorageFiles => Set<StorageFile>();

    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new
            {
                ur.UserId,
                ur.RoleId
            });

        /// user - userrole
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        /// role - userrole
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        ///user - storagefile
        modelBuilder.Entity<User>()
            .HasOne(u => u.AvatarFile)
            .WithMany()
            .HasForeignKey(u => u.AvatarFileId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<RevokedToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RevokedTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.NoAction);



        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TokenHash)
                .HasMaxLength(64)
                .IsRequired();

            entity.HasIndex(x => x.TokenHash)
                .IsUnique();

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ReplacedByToken)
                .WithMany()
                .HasForeignKey(x => x.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
