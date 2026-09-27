using System;
using WebsiteQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel.Args;
using WebsiteQuanLyThuVien.Enums;

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

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Publisher> Publishers => Set<Publisher>();

    public DbSet<Book> Books => Set<Book>();

    public DbSet<BookCategory> BookCategories => Set<BookCategory>();

    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();

    public DbSet<Author> Authors => Set<Author>();

    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();

    public DbSet<BookCopy> BookCopies => Set<BookCopy>();

    public DbSet<UploadSession> UploadSessions => Set<UploadSession>();

    public DbSet<CardRegistration> CardRegistrations => Set<CardRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new
            {
                ur.UserId,
                ur.RoleId
            });

        modelBuilder.Entity<User>(entity =>
            {
                entity.Property(x => x.Status)
                    .HasConversion<string>();
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


        //book
        modelBuilder.Entity<Book>(entity =>
       {
           entity.ToTable("books");

           entity.HasKey(x => x.Id);

           entity.Property(x => x.Id)
               .HasColumnName("id")
               .ValueGeneratedOnAdd();

           entity.Property(x => x.Isbn)
               .HasColumnName("isbn")
               .HasMaxLength(20);

           entity.HasIndex(x => x.Isbn)
               .IsUnique();

           entity.Property(x => x.Title)
               .HasColumnName("title")
               .HasMaxLength(500)
               .IsRequired();

           entity.Property(x => x.Description)
               .HasColumnName("description");

           entity.Property(x => x.PublicationYear)
               .HasColumnName("publication_year");

           entity.Property(x => x.Language)
               .HasColumnName("language")
               .HasMaxLength(50);

           entity.Property(x => x.PageCount)
               .HasColumnName("page_count");

           entity.Property(x => x.Price)
               .HasColumnName("price")
               .HasPrecision(12, 2);

           entity.Property(x => x.Status)
               .HasColumnName("status")
               .HasConversion<string>()
               .HasMaxLength(20)
               .HasDefaultValue(BookStatus.ACTIVE)
               .IsRequired();

           entity.Property(x => x.PublisherId)
               .HasColumnName("publisher_id");

           entity.Property(x => x.DocumentTypeId)
               .HasColumnName("document_type_id");

           entity.Property(x => x.CreatedAt)
               .HasColumnName("created_at")
               .HasDefaultValueSql("SYSUTCDATETIME()");

           entity.Property(x => x.UpdatedAt)
               .HasColumnName("updated_at")
               .HasDefaultValueSql("SYSUTCDATETIME()");

           entity.HasOne(x => x.Publisher)
               .WithMany(x => x.Books)
               .HasForeignKey(x => x.PublisherId)
               .OnDelete(DeleteBehavior.SetNull);

           entity.HasOne(x => x.DocumentType)
               .WithMany(x => x.Books)
               .HasForeignKey(x => x.DocumentTypeId)
               .OnDelete(DeleteBehavior.SetNull);
       });




        //Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasColumnName("description");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(x => x.DisplayOrder)
                .HasColumnName("display_order")
                .HasDefaultValue(0);
        });



        //BookCategory    
        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.ToTable("book_categories");

            entity.HasKey(x => new
            {
                x.BookId,
                x.CategoryId
            });

            entity.Property(x => x.BookId)
                .HasColumnName("book_id");

            entity.Property(x => x.CategoryId)
                .HasColumnName("category_id");

            entity.HasOne(x => x.Book)
                .WithMany(x => x.BookCategories)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.BookCategories)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        //Publisher
        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("publishers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.Property(x => x.Address)
                .HasColumnName("address");

            entity.Property(x => x.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            entity.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(255);

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("SYSUTCDATETIME()");
        });



        //DocumentType
        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("document_types");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.Property(x => x.Description)
                .HasColumnName("description");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("SYSUTCDATETIME()");
        });


        //Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();

            entity.Property(x => x.Biography).HasColumnType("nvarchar(max)");

            entity.Property(x => x.CreatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(x => x.UpdatedAt).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
        });


        //BookAuthor
        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.ToTable("book_authors");

            entity.HasKey(x => new
            {
                x.BookId,
                x.AuthorId
            });

            entity.Property(x => x.BookId)
                .HasColumnName("book_id");

            entity.Property(x => x.AuthorId)
                .HasColumnName("author_id");

            entity.HasOne(x => x.Book)
                .WithMany(x => x.BookAuthors)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Author)
                .WithMany(x => x.BookAuthors)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        //BookCopy
        modelBuilder.Entity<BookCopy>(entity =>
       {
           entity.ToTable("book_copies");

           entity.HasKey(x => x.Id);

           entity.Property(x => x.Id)
               .HasDefaultValueSql("NEWSEQUENTIALID()");

           entity.Property(x => x.BookId)
               .IsRequired();

           entity.Property(x => x.LocationId)
               .IsRequired(false);

           entity.Property(x => x.Barcode)
               .HasMaxLength(100)
               .IsRequired();

           entity.HasIndex(x => x.Barcode)
               .IsUnique();

           entity.Property(x => x.Status)
              .HasConversion<string>()
              .HasMaxLength(20)
              .HasDefaultValue(BookCopyStatus.AVAILABLE)
              .IsRequired();

           entity.Property(x => x.Condition)
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();

           entity.Property(x => x.AcquiredAt)
               .HasColumnType("date");

           entity.Property(x => x.AcquisitionPrice)
               .HasColumnType("decimal(12,2)");

           entity.Property(x => x.CreatedAt)
               .HasColumnType("datetime2(7)")
               .HasDefaultValueSql("SYSUTCDATETIME()");

           entity.Property(x => x.UpdatedAt)
               .HasColumnType("datetime2(7)")
               .HasDefaultValueSql("SYSUTCDATETIME()");


           entity.HasOne(x => x.Book)
               .WithMany(x => x.BookCopies)
               .HasForeignKey(x => x.BookId)
               .OnDelete(DeleteBehavior.Restrict);

           // entity.HasOne(x => x.Location)
           //     .WithMany()
           //     .HasForeignKey(x => x.LocationId)
           //     .OnDelete(DeleteBehavior.SetNull);
       });


        //UploadSession
        modelBuilder.Entity<UploadSession>(entity =>
        {
            entity.ToTable("upload_sessions");

            entity.HasKey(x => x.Id)
                .HasName("PK_upload_sessions");

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.CardRegistrationId)
                .HasColumnName("card_registration_id");

            entity.Property(x => x.UserId)
                .HasColumnName("user_id");

            entity.Property(x => x.ObjectKey)
                .HasColumnName("object_key")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.UploadType)
                .HasColumnName("upload_type")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(x => x.CompletedAt)
                .HasColumnName("completed_at");

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_upload_sessions_user")
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.CardRegistration)
                .WithMany()
                .HasForeignKey(x => x.CardRegistrationId)
                .HasConstraintName("fk_upload_session_registration")
                .OnDelete(DeleteBehavior.NoAction);
        });

        //CardRegistration
        modelBuilder.Entity<CardRegistration>(entity =>
        {
            entity.ToTable("card_registrations");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.IdDocumentNumber)
                .HasColumnName("id_document_number")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.IdDocumentFrontFileId)
                .HasColumnName("id_document_front_file_id");

            entity.Property(x => x.IdDocumentBackFileId)
                .HasColumnName("id_document_back_file_id");

            entity.Property(x => x.AvatarFileId)
                .HasColumnName("avatar_file_id");

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(CardRegistrationStatus.WAITING_FOR_INFORMATION)
                .IsRequired();

            entity.Property(x => x.SubmittedAt)
                .HasColumnName("submitted_at")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            entity.Property(x => x.ReviewedAt)
                .HasColumnName("reviewed_at");

            entity.Property(x => x.ReviewedBy)
                .HasColumnName("reviewed_by");

            entity.Property(x => x.RejectionReason)
                .HasColumnName("rejection_reason");

            entity.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            entity.HasOne(x => x.Reviewer)
                .WithMany()
                .HasForeignKey(x => x.ReviewedBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.AvatarFile)
                .WithMany()
                .HasForeignKey(x => x.AvatarFileId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.IdDocumentFrontFile)
                .WithMany()
                .HasForeignKey(x => x.IdDocumentFrontFileId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.IdDocumentBackFile)
                .WithMany()
                .HasForeignKey(x => x.IdDocumentBackFileId)
                .OnDelete(DeleteBehavior.NoAction);
        });

    }
}
