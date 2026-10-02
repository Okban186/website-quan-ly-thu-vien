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

    public DbSet<StorageFile> StorageFiles => Set<StorageFile>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<LibraryMember> LibraryMembers => Set<LibraryMember>();
    public DbSet<LibraryCard> LibraryCards => Set<LibraryCard>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<LibraryItem> LibraryItems => Set<LibraryItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ResourceCategory> ResourceCategories => Set<ResourceCategory>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<ResourceAuthor> ResourceAuthors => Set<ResourceAuthor>();
    public DbSet<BorrowRequest> BorrowRequests => Set<BorrowRequest>();
    public DbSet<BorrowRequestItem> BorrowRequestItems => Set<BorrowRequestItem>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanItem> LoanItems => Set<LoanItem>();
    public DbSet<LoanRenewal> LoanRenewals => Set<LoanRenewal>();
    public DbSet<Charge> Charges => Set<Charge>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentItem> PaymentItems => Set<PaymentItem>();
    public DbSet<CardRegistration> CardRegistrations => Set<CardRegistration>();
    public DbSet<ResourceImage> ResourceImages => Set<ResourceImage>();
    public DbSet<DigitalResource> DigitalResources => Set<DigitalResource>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<CopyIssue> CopyIssues => Set<CopyIssue>();
    public DbSet<LibraryItemStatusHistory> LibraryItemStatusHistories => Set<LibraryItemStatusHistory>();
    public DbSet<UploadSession> UploadSessions => Set<UploadSession>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureStorageFile(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureUserRole(modelBuilder);
        ConfigureStaff(modelBuilder);
        ConfigureLibraryMember(modelBuilder);
        ConfigureLibraryCard(modelBuilder);
        ConfigurePublisher(modelBuilder);
        ConfigureLocation(modelBuilder);
        ConfigureDocumentType(modelBuilder);
        ConfigureResource(modelBuilder);
        ConfigureLibraryItem(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigureResourceCategory(modelBuilder);
        ConfigureAuthor(modelBuilder);
        ConfigureResourceAuthor(modelBuilder);
        ConfigureBorrowRequest(modelBuilder);
        ConfigureBorrowRequestItem(modelBuilder);
        ConfigureDelivery(modelBuilder);
        ConfigureLoan(modelBuilder);
        ConfigureLoanItem(modelBuilder);
        ConfigureLoanRenewal(modelBuilder);
        ConfigureCharge(modelBuilder);
        ConfigurePaymentTransaction(modelBuilder);
        ConfigurePaymentItem(modelBuilder);
        ConfigureCardRegistration(modelBuilder);
        ConfigureResourceImage(modelBuilder);
        ConfigureDigitalResource(modelBuilder);
        ConfigureNotification(modelBuilder);
        ConfigureNotificationRecipient(modelBuilder);
        ConfigureCopyIssue(modelBuilder);
        ConfigureLibraryItemStatusHistory(modelBuilder);
        ConfigureUploadSession(modelBuilder);
        ConfigureRefreshToken(modelBuilder);
        ConfigureRevokedToken(modelBuilder);
    }

    private static void ConfigureStorageFile(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StorageFile>(entity =>
        {
            entity.ToTable("storage_files");

            entity.HasKey(x => x.Id).HasName("PK_storage_files");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.ContentType).HasColumnName("content_type").HasMaxLength(100);
            entity.Property(x => x.FileSize).HasColumnName("file_size");
            entity.Property(x => x.ObjectKey).HasColumnName("object_key").HasMaxLength(500).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.ObjectKey).IsUnique().HasDatabaseName("UX_storage_files_object_key");
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(x => x.Id).HasName("PK_users");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(500).IsRequired();
            entity.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
            entity.Property(x => x.DateOfBirth).HasColumnName("date_of_birth").HasColumnType("date");
            entity.Property(x => x.CitizenId).HasColumnName("citizen_id").HasMaxLength(20);
            entity.Property(x => x.Gender).HasColumnName("gender").HasMaxLength(20);
            entity.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
            entity.Property(x => x.AvatarFileId).HasColumnName("avatar_file_id");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).HasConversion<string>().HasDefaultValue(UserStatus.ACTIVE);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Username).IsUnique().HasDatabaseName("UQ_users_username");
            entity.HasIndex(x => x.Email).IsUnique().HasDatabaseName("UQ_users_email");
            entity.HasIndex(x => x.CitizenId).IsUnique().HasDatabaseName("UQ_users_citizen_id");

            entity.HasOne(x => x.AvatarFile).WithOne(x => x.AvatarUser).HasForeignKey<User>(x => x.AvatarFileId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_users_avatar_file");
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(x => x.Id).HasName("PK_roles");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);

            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UQ_roles_name");
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");

            entity.HasKey(x => new { x.UserId, x.RoleId }).HasName("PK_user_roles");

            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.RoleId).HasColumnName("role_id");

            entity.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_user_roles_user");
            entity.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_user_roles_role");
        });
    }

    private static void ConfigureStaff(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");

            entity.HasKey(x => x.Id).HasName("PK_staff");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.EmployeeCode).HasColumnName("employee_code").HasMaxLength(50).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("UQ_staff_user_id");
            entity.HasIndex(x => x.EmployeeCode).IsUnique().HasDatabaseName("UQ_staff_employee_code");

            entity.HasOne(x => x.User).WithOne(x => x.Staff).HasForeignKey<Staff>(x => x.UserId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_staff_user");
        });
    }

    private static void ConfigureLibraryMember(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryMember>(entity =>
        {
            entity.ToTable("library_members");

            entity.HasKey(x => x.Id).HasName("PK_library_members");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.MembershipStatus).HasColumnName("membership_status").HasMaxLength(30).HasConversion<string>().HasDefaultValue(LibraryMembershipStatus.ACTIVE);
            entity.Property(x => x.RegisteredAt).HasColumnName("registered_at").HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.ExpiredAt).HasColumnName("expired_at").HasColumnType("datetime2");

            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("UQ_library_members_user_id");
            entity.HasOne(x => x.User).WithOne(x => x.LibraryMember).HasForeignKey<LibraryMember>(x => x.UserId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_library_members_user");
        });
    }

    private static void ConfigureLibraryCard(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryCard>(entity =>
        {
            entity.ToTable("library_cards");

            entity.HasKey(x => x.Id).HasName("PK_library_cards");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.MemberId).HasColumnName("member_id");
            entity.Property(x => x.CardNumber).HasColumnName("card_number").HasMaxLength(50).IsRequired();
            entity.Property(x => x.IssuedAt).HasColumnName("issued_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.ExpiredAt).HasColumnName("expired_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(LibraryCardStatus.PENDING);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.MemberId).IsUnique().HasDatabaseName("UQ_library_cards_member");
            entity.HasIndex(x => x.CardNumber).IsUnique().HasDatabaseName("UQ_library_cards_number");
            entity.HasOne(x => x.Member).WithOne(x => x.LibraryCard).HasForeignKey<LibraryCard>(x => x.MemberId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_library_cards_member");

            entity.ToTable(t => { t.HasCheckConstraint("CK_library_cards_status", "[status] IN ('PENDING','ACTIVE','BLOCKED','EXPIRED','CANCELLED')"); });
        });
    }

    private static void ConfigureImportBatch(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImportBatch>(entity =>
        {
            entity.ToTable("import_batches");

            entity.HasKey(x => x.Id)
                .HasName("PK_import_batches");

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.StorageFileId)
                .HasColumnName("storage_file_id")
                .IsRequired();

            entity.Property(x => x.ImportType)
                .HasColumnName("import_type")
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue("LIBRARY_ITEM");

            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasMaxLength(30)
                .IsRequired()
                .HasDefaultValue("PENDING");

            entity.Property(x => x.TotalRows)
                .HasColumnName("total_rows")
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(x => x.SuccessRows)
                .HasColumnName("success_rows")
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(x => x.FailedRows)
                .HasColumnName("failed_rows")
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime2(7)")
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(x => x.StartedAt)
                .HasColumnName("started_at")
                .HasColumnType("datetime2(7)");

            entity.Property(x => x.CompletedAt)
                .HasColumnName("completed_at")
                .HasColumnType("datetime2(7)");

            entity.Property(x => x.ErrorMessage)
                .HasColumnName("error_message");

            entity.HasOne(x => x.StorageFile)
                .WithMany()
                .HasForeignKey(x => x.StorageFileId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_import_batches_storage_file");

            entity.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_import_batches_created_by");

            entity.HasIndex(x => x.StorageFileId)
                .HasDatabaseName("IX_import_batches_storage_file_id");

            entity.HasIndex(x => x.CreatedBy)
                .HasDatabaseName("IX_import_batches_created_by");

            entity.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("IX_import_batches_created_at");

            entity.HasIndex(x => x.Status)
                .HasDatabaseName("IX_import_batches_status");
        });
    }

    private static void ConfigureResource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.ToTable("resources");

            entity.HasKey(x => x.Id).HasName("PK_resources");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Isbn).HasColumnName("isbn").HasMaxLength(20);
            entity.Property(x => x.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description");
            entity.Property(x => x.PublicationYear).HasColumnName("publication_year");
            entity.Property(x => x.Language).HasColumnName("language").HasMaxLength(50);
            entity.Property(x => x.PageCount).HasColumnName("page_count");
            entity.Property(x => x.Price).HasColumnName("price").HasPrecision(12, 2);
            entity.Property(x => x.PublisherId).HasColumnName("publisher_id");
            entity.Property(x => x.DocumentTypeId).HasColumnName("document_type_id");
            entity.Property(x => x.PhysicalDescription).HasColumnName("physical_description").HasMaxLength(1000);
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(ResourceStatus.ACTIVE);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Isbn).IsUnique().HasDatabaseName("UQ_resources_isbn");
            entity.HasIndex(x => x.Title).HasDatabaseName("IX_resources_title");
            entity.HasIndex(x => x.DocumentTypeId).HasDatabaseName("IX_resources_document_type_id");
            entity.HasIndex(x => x.PublisherId).HasDatabaseName("IX_resources_publisher_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_resources_status");

            entity.HasOne(x => x.Publisher).WithMany(x => x.Resources).HasForeignKey(x => x.PublisherId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_resources_publisher");
            entity.HasOne(x => x.DocumentType).WithMany(x => x.Resources).HasForeignKey(x => x.DocumentTypeId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_resources_document_type");

            entity.ToTable(t => { t.HasCheckConstraint("CK_resources_status", "[status] IN ('ACTIVE','INACTIVE')"); });
        });
    }

    private static void ConfigureLibraryItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryItem>(entity =>
        {
            entity.ToTable("library_items");

            entity.HasKey(x => x.Id).HasName("PK_library_items");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.ResourceId).HasColumnName("resource_id");
            entity.Property(x => x.LocationId).HasColumnName("location_id");
            entity.Property(x => x.Barcode).HasColumnName("barcode").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(LibraryItemStatus.AVAILABLE);
            entity.Property(x => x.ItemCondition).HasColumnName("item_condition").HasMaxLength(20).HasConversion<string>().HasDefaultValue(ItemCondition.GOOD);
            entity.Property(x => x.AcquiredAt).HasColumnName("acquired_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.AcquisitionPrice).HasColumnName("acquisition_price").HasPrecision(12, 2);
            entity.Property(x => x.CoverPrice).HasColumnName("cover_price").HasPrecision(12, 2);
            entity.Property(x => x.PublicationYear).HasColumnName("publication_year");
            entity.Property(x => x.EditionNumber).HasColumnName("edition_number");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Barcode).IsUnique().HasDatabaseName("UQ_library_items_barcode");
            entity.HasIndex(x => x.ResourceId).HasDatabaseName("IX_library_items_resource_id");
            entity.HasIndex(x => new { x.ResourceId, x.Status }).HasDatabaseName("IX_library_items_resource_status");
            entity.HasIndex(x => x.LocationId).HasDatabaseName("IX_library_items_location_id");

            entity.HasOne(x => x.Resource).WithMany(x => x.LibraryItems).HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_library_items_resource");
            entity.HasOne(x => x.Location).WithMany(x => x.LibraryItems).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_library_items_location");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_library_items_status", "[status] IN ('AVAILABLE','RESERVED','BORROWED','LOST','DAMAGED','MAINTENANCE','REMOVED')");
                t.HasCheckConstraint("CK_library_items_condition", "[item_condition] IN ('NEW','GOOD','WORN','DAMAGED')");
            });
        });
    }

    private static void ConfigureBorrowRequest(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BorrowRequest>(entity =>
        {
            entity.ToTable("borrow_requests");

            entity.HasKey(x => x.Id).HasName("PK_borrow_requests");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.MemberId).HasColumnName("member_id");
            entity.Property(x => x.RequestType).HasColumnName("request_type").HasMaxLength(20).HasConversion<string>().IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).HasConversion<string>().HasDefaultValue(BorrowRequestStatus.PENDING);
            entity.Property(x => x.ProcessedBy).HasColumnName("processed_by");
            entity.Property(x => x.Note).HasColumnName("note").HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.MemberId).HasDatabaseName("IX_borrow_requests_member_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_borrow_requests_status");

            entity.HasOne(x => x.Member).WithMany(x => x.BorrowRequests).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_borrow_requests_member");
            entity.HasOne(x => x.Processor).WithMany(x => x.ProcessedBorrowRequests).HasForeignKey(x => x.ProcessedBy).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_borrow_requests_processed_by");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_borrow_requests_type", "[request_type] IN ('RESERVATION','DELIVERY')");
                t.HasCheckConstraint("CK_borrow_requests_status", "[status] IN ('PENDING','APPROVED','READY_FOR_PICKUP','DELIVERING','COMPLETED','REJECTED','CANCELLED','EXPIRED')");
            });
        });
    }

    private static void ConfigureBorrowRequestItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BorrowRequestItem>(entity =>
        {
            entity.ToTable("borrow_request_items");

            entity.HasKey(x => x.Id).HasName("PK_borrow_request_items");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.BorrowRequestId).HasColumnName("borrow_request_id");
            entity.Property(x => x.ResourceId).HasColumnName("resource_id");
            entity.Property(x => x.LibraryItemId).HasColumnName("library_item_id");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).HasConversion<string>().HasDefaultValue(BorrowRequestItemStatus.PENDING);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.BorrowRequestId).HasDatabaseName("IX_borrow_request_items_request_id");
            entity.HasIndex(x => x.ResourceId).HasDatabaseName("IX_borrow_request_items_resource_id");
            entity.HasIndex(x => x.LibraryItemId).HasDatabaseName("IX_borrow_request_items_library_item_id");

            entity.HasOne(x => x.BorrowRequest).WithMany(x => x.Items).HasForeignKey(x => x.BorrowRequestId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_borrow_request_items_request");
            entity.HasOne(x => x.Resource).WithMany(x => x.BorrowRequestItems).HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_borrow_request_items_resource");
            entity.HasOne(x => x.LibraryItem).WithMany(x => x.BorrowRequestItems).HasForeignKey(x => x.LibraryItemId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_borrow_request_items_library_item");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_borrow_request_items_status", "[status] IN ('PENDING','APPROVED','COMPLETED','REJECTED','CANCELLED')");
            });
        });
    }


    private static void ConfigureDelivery(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.ToTable("deliveries");

            entity.HasKey(x => x.Id).HasName("PK_deliveries");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.BorrowRequestId).HasColumnName("borrow_request_id");
            entity.Property(x => x.Address).HasColumnName("address").HasMaxLength(500).IsRequired();
            entity.Property(x => x.RecipientName).HasColumnName("recipient_name").HasMaxLength(255).IsRequired();
            entity.Property(x => x.RecipientPhone).HasColumnName("recipient_phone").HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).HasConversion<string>().HasDefaultValue(DeliveryStatus.PENDING);
            entity.Property(x => x.DeliveredAt).HasColumnName("delivered_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.BorrowRequestId).IsUnique().HasDatabaseName("UQ_deliveries_borrow_request");
            entity.HasOne(x => x.BorrowRequest).WithOne(x => x.Delivery).HasForeignKey<Delivery>(x => x.BorrowRequestId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_deliveries_borrow_request");
        });
    }

    private static void ConfigureLoan(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("loans");

            entity.HasKey(x => x.Id).HasName("PK_loans");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.MemberId).HasColumnName("member_id");
            entity.Property(x => x.BorrowRequestId).HasColumnName("borrow_request_id");
            entity.Property(x => x.CreatedBy).HasColumnName("created_by");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(LoanStatus.ACTIVE);
            entity.Property(x => x.BorrowedAt).HasColumnName("borrowed_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.MemberId).HasDatabaseName("IX_loans_member_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_loans_status");

            entity.HasOne(x => x.Member).WithMany(x => x.Loans).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_loans_member");
            entity.HasOne(x => x.BorrowRequest).WithMany(x => x.Loans).HasForeignKey(x => x.BorrowRequestId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_loans_borrow_request");
            entity.HasOne(x => x.Creator).WithMany(x => x.CreatedLoans).HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_loans_created_by");

            entity.ToTable(t => { t.HasCheckConstraint("CK_loans_status", "[status] IN ('ACTIVE','COMPLETED','CANCELLED')"); });
        });
    }

    private static void ConfigureLoanItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoanItem>(entity =>
        {
            entity.ToTable("loan_items");

            entity.HasKey(x => x.Id).HasName("PK_loan_items");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.LoanId).HasColumnName("loan_id");
            entity.Property(x => x.LibraryItemId).HasColumnName("library_item_id");
            entity.Property(x => x.DueAt).HasColumnName("due_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.ReturnedAt).HasColumnName("returned_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(LoanItemStatus.BORROWED);
            entity.Property(x => x.ConditionAtLoan).HasColumnName("condition_at_loan").HasMaxLength(20).HasConversion<string>();
            entity.Property(x => x.ConditionAtReturn).HasColumnName("condition_at_return").HasMaxLength(20).HasConversion<string>();
            entity.Property(x => x.ConditionNoteAtLoan).HasColumnName("condition_note_at_loan");
            entity.Property(x => x.ConditionNoteAtReturn).HasColumnName("condition_note_at_return");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.LoanId).HasDatabaseName("IX_loan_items_loan_id");
            entity.HasIndex(x => x.LibraryItemId).HasDatabaseName("IX_loan_items_library_item_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_loan_items_status");
            entity.HasIndex(x => x.DueAt).HasDatabaseName("IX_loan_items_due_at");

            entity.HasOne(x => x.Loan).WithMany(x => x.LoanItems).HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_loan_items_loan");
            entity.HasOne(x => x.LibraryItem).WithMany(x => x.LoanItems).HasForeignKey(x => x.LibraryItemId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_loan_items_library_item");

            entity.ToTable(t => { t.HasCheckConstraint("CK_loan_items_status", "[status] IN ('BORROWED','OVERDUE','RETURNED','LOST')"); });
        });
    }

    private static void ConfigureLoanRenewal(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoanRenewal>(entity =>
        {
            entity.ToTable("loan_renewals");

            entity.HasKey(x => x.Id).HasName("PK_loan_renewals");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.LoanItemId).HasColumnName("loan_item_id");
            entity.Property(x => x.OldDueAt).HasColumnName("old_due_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.NewDueAt).HasColumnName("new_due_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.RenewedBy).HasColumnName("renewed_by");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(x => x.LoanItem).WithMany(x => x.Renewals).HasForeignKey(x => x.LoanItemId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_loan_renewals_loan_item");
            entity.HasOne(x => x.RenewedByUser).WithMany(x => x.LoanRenewals).HasForeignKey(x => x.RenewedBy).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_loan_renewals_renewed_by");
        });
    }

    private static void ConfigureCharge(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Charge>(entity =>
        {
            entity.ToTable("charges");

            entity.HasKey(x => x.Id).HasName("PK_charges");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.MemberId).HasColumnName("member_id");
            entity.Property(x => x.LoanItemId).HasColumnName("loan_item_id");
            entity.Property(x => x.ChargeType).HasColumnName("charge_type").HasMaxLength(20).HasConversion<string>();
            entity.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2);
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(ChargeStatus.UNPAID);
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.MemberId).HasDatabaseName("IX_charges_member_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_charges_status");

            entity.HasOne(x => x.Member).WithMany(x => x.Charges).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_charges_member");
            entity.HasOne(x => x.LoanItem).WithMany(x => x.Charges).HasForeignKey(x => x.LoanItemId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_charges_loan_item");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_charges_type", "[charge_type] IN ('CARD_FEE','OVERDUE','DAMAGED','LOST')");
                t.HasCheckConstraint("CK_charges_status", "[status] IN ('UNPAID','PAID','WAIVED')");
                t.HasCheckConstraint("CK_charges_amount", "[amount] >= 0");
            });
        });
    }

    private static void ConfigurePaymentTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.ToTable("payment_transactions");

            entity.HasKey(x => x.Id).HasName("PK_payment_transactions");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.MemberId).HasColumnName("member_id");
            entity.Property(x => x.TransactionCode).HasColumnName("transaction_code").HasMaxLength(100).IsRequired();
            entity.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasMaxLength(30).HasConversion<string>().HasDefaultValue(PaymentMethod.ONLINE_PAYMENT);
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(PaymentTransactionStatus.PENDING);
            entity.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(12, 2);
            entity.Property(x => x.ExpiredAt).HasColumnName("expired_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.TransactionCode).IsUnique().HasDatabaseName("UQ_payment_transactions_code");
            entity.HasIndex(x => x.MemberId).HasDatabaseName("IX_payment_transactions_member_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_payment_transactions_status");

            entity.HasOne(x => x.Member).WithMany(x => x.PaymentTransactions).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_payment_transactions_member");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_payment_transactions_method", "[payment_method] IN ('ONLINE_PAYMENT')");
                t.HasCheckConstraint("CK_payment_transactions_status", "[status] IN ('PENDING','PROCESSING','PAID','FAILED','CANCELLED')");
                t.HasCheckConstraint("CK_payment_transactions_amount", "[total_amount] >= 0");
            });
        });
    }

    private static void ConfigurePaymentItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentItem>(entity =>
        {
            entity.ToTable("payment_items");

            entity.HasKey(x => x.Id).HasName("PK_payment_items");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.PaymentId).HasColumnName("payment_id");
            entity.Property(x => x.ChargeId).HasColumnName("charge_id");
            entity.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => new { x.PaymentId, x.ChargeId }).IsUnique().HasDatabaseName("UQ_payment_items_payment_charge");
            entity.HasIndex(x => x.ChargeId).HasDatabaseName("IX_payment_items_charge_id");

            entity.HasOne(x => x.Payment).WithMany(x => x.PaymentItems).HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_payment_items_payment");
            entity.HasOne(x => x.Charge).WithMany(x => x.PaymentItems).HasForeignKey(x => x.ChargeId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_payment_items_charge");

            entity.ToTable(t => { t.HasCheckConstraint("CK_payment_items_amount", "[amount] >= 0"); });
        });
    }

    private static void ConfigureCardRegistration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardRegistration>(entity =>
        {
            entity.ToTable("card_registrations");

            entity.HasKey(x => x.Id).HasName("PK_card_registrations");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();

            // Missing foreign key mappings added here
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.PaymentTransactionId).HasColumnName("payment_transaction_id");
            entity.Property(x => x.IdDocumentFrontFileId).HasColumnName("id_document_front_file_id");
            entity.Property(x => x.IdDocumentBackFileId).HasColumnName("id_document_back_file_id");
            entity.Property(x => x.AvatarFileId).HasColumnName("avatar_file_id");
            entity.Property(x => x.ReviewedBy).HasColumnName("reviewed_by");

            entity.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(255);
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30);
            entity.Property(x => x.IdDocumentNumber).HasColumnName("id_document_number").HasMaxLength(100);
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).HasConversion<string>().HasDefaultValue(CardRegistrationStatus.WAITING_FOR_INFORMATION);
            entity.Property(x => x.RegistrationTokenHash).HasColumnName("registration_token_hash").HasMaxLength(64);

            // Added missing columns mapping
            entity.Property(x => x.RegistrationTokenExpiredAt).HasColumnName("registration_token_expired_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.RegistrationTokenRevokedAt).HasColumnName("registration_token_revoked_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.SubmittedAt).HasColumnName("submitted_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.ReviewedAt).HasColumnName("reviewed_at").HasColumnType("datetime2(7)");

            entity.Property(x => x.RejectionReason).HasColumnName("rejection_reason").HasMaxLength(1000);

            entity.HasIndex(x => x.PaymentTransactionId).IsUnique().HasDatabaseName("UQ_card_registrations_payment");
            entity.HasIndex(x => x.RegistrationTokenHash).IsUnique().HasDatabaseName("UQ_card_registrations_token");
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_card_registrations_user_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_card_registrations_status");

            entity.HasOne(x => x.User).WithMany(x => x.CardRegistrations).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_card_registrations_user");
            entity.HasOne(x => x.PaymentTransaction).WithOne(x => x.CardRegistration).HasForeignKey<CardRegistration>(x => x.PaymentTransactionId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_card_registrations_payment");
            entity.HasOne(x => x.IdDocumentFrontFile).WithMany(x => x.DocumentFrontRegistrations).HasForeignKey(x => x.IdDocumentFrontFileId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_card_registrations_front_file");
            entity.HasOne(x => x.IdDocumentBackFile).WithMany(x => x.DocumentBackRegistrations).HasForeignKey(x => x.IdDocumentBackFileId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_card_registrations_back_file");
            entity.HasOne(x => x.AvatarFile).WithMany(x => x.AvatarRegistrations).HasForeignKey(x => x.AvatarFileId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_card_registrations_avatar_file");
            entity.HasOne(x => x.Reviewer).WithMany(x => x.ReviewedCardRegistrations).HasForeignKey(x => x.ReviewedBy).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_card_registrations_reviewer");

            entity.ToTable(t => { t.HasCheckConstraint("CK_card_registrations_status", "[status] IN ('WAITING_FOR_INFORMATION','PENDING','APPROVED','REJECTED','CANCELLED','EXPIRED')"); });
        });
    }

    private static void ConfigureResourceImage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResourceImage>(entity =>
        {
            entity.ToTable("resource_images");

            entity.HasKey(x => x.Id).HasName("PK_resource_images");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.ResourceId).HasColumnName("resource_id");
            entity.Property(x => x.FileId).HasColumnName("file_id");
            entity.Property(x => x.IsPrimary).HasColumnName("is_primary").HasDefaultValue(false);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.ResourceId).HasDatabaseName("IX_resource_images_resource_id");
            entity.HasIndex(x => x.ResourceId).IsUnique().HasFilter("[is_primary] = 1").HasDatabaseName("UX_resource_images_primary");

            entity.HasOne(x => x.Resource).WithMany(x => x.ResourceImages).HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_resource_images_resource");
            entity.HasOne(x => x.File).WithMany(x => x.ResourceImages).HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_resource_images_file");
        });
    }

    private static void ConfigureDigitalResource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DigitalResource>(entity =>
        {
            entity.ToTable("digital_resources");

            entity.HasKey(x => x.Id).HasName("PK_digital_resources");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.ResourceId).HasColumnName("resource_id");
            entity.Property(x => x.FileId).HasColumnName("file_id");
            entity.Property(x => x.DisplayOrder).HasColumnName("display_order").HasDefaultValue(1);
            entity.Property(x => x.ResourceType).HasColumnName("resource_type").HasMaxLength(30).HasConversion<string>();
            entity.Property(x => x.AccessLevel).HasColumnName("access_level").HasMaxLength(20).HasConversion<string>().HasDefaultValue(DigitalResourceAccessLevel.MEMBER);
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(DigitalResourceStatus.ACTIVE);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.ResourceId).HasDatabaseName("IX_digital_resources_resource_id");
            entity.HasIndex(x => x.ResourceType).HasDatabaseName("IX_digital_resources_resource_type");
            entity.HasIndex(r => new { r.ResourceId, r.ResourceType, r.DisplayOrder }).IsUnique().HasDatabaseName("UQ_digital_resources_order");
            entity.HasOne(x => x.Resource).WithMany(x => x.DigitalResources).HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_digital_resources_resource");
            entity.HasOne(x => x.File).WithMany(x => x.DigitalResources).HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_digital_resources_file");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_digital_resources_type", "[resource_type] IN ('EBOOK','DOCUMENT','AUDIOBOOK','OTHER')");
                t.HasCheckConstraint("CK_digital_resources_access_level", "[access_level] IN ('PUBLIC','MEMBER','STAFF','HIDDEN')");
                t.HasCheckConstraint("CK_digital_resources_status", "[status] IN ('ACTIVE','INACTIVE')");
            });
        });
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");

            entity.HasKey(x => x.Id).HasName("PK_notifications");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(x => x.Message).HasColumnName("message").HasMaxLength(1000).IsRequired();
            entity.Property(x => x.NotificationType).HasColumnName("notification_type").HasMaxLength(50);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
        });
    }

    private static void ConfigureNotificationRecipient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationRecipient>(entity =>
        {
            entity.ToTable("notification_recipients");

            entity.HasKey(x => new { x.NotificationId, x.UserId }).HasName("PK_notification_recipients");

            entity.Property(x => x.NotificationId).HasColumnName("notification_id");
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.IsRead).HasColumnName("is_read").HasDefaultValue(false);
            entity.Property(x => x.ReadAt).HasColumnName("read_at").HasColumnType("datetime2");

            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_notification_recipients_user_id");
            entity.HasIndex(x => new { x.UserId, x.IsRead }).HasDatabaseName("IX_notification_recipients_user_unread");

            entity.HasOne(x => x.Notification).WithMany(x => x.Recipients).HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_notification_recipients_notification");
            entity.HasOne(x => x.User).WithMany(x => x.NotificationRecipients).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_notification_recipients_user");
        });
    }

    private static void ConfigureCopyIssue(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CopyIssue>(entity =>
        {
            entity.ToTable("copy_issues");

            entity.HasKey(x => x.Id).HasName("PK_copy_issues");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.LibraryItemId).HasColumnName("library_item_id");
            entity.Property(x => x.LoanItemId).HasColumnName("loan_item_id");
            entity.Property(x => x.ReportedBy).HasColumnName("reported_by");
            entity.Property(x => x.ResolvedBy).HasColumnName("resolved_by");
            entity.Property(x => x.IssueType).HasColumnName("issue_type").HasMaxLength(30).HasConversion<string>();
            entity.Property(x => x.Description).HasColumnName("description");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(CopyIssueStatus.OPEN);
            entity.Property(x => x.ResolvedAt).HasColumnName("resolved_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.LibraryItemId).HasDatabaseName("IX_copy_issues_library_item_id");
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_copy_issues_status");

            entity.HasOne(x => x.LibraryItem).WithMany(x => x.CopyIssues).HasForeignKey(x => x.LibraryItemId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_copy_issues_library_item");
            entity.HasOne(x => x.Reporter).WithMany(x => x.ReportedCopyIssues).HasForeignKey(x => x.ReportedBy).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_copy_issues_reported_by");
            entity.HasOne(x => x.Resolver).WithMany(x => x.ResolvedCopyIssues).HasForeignKey(x => x.ResolvedBy).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_copy_issues_resolved_by");

            entity.ToTable(t => { t.HasCheckConstraint("CK_copy_issues_status", "[status] IN ('OPEN','IN_PROGRESS','RESOLVED','CANCELLED')"); });
        });
    }

    private static void ConfigureLibraryItemStatusHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryItemStatusHistory>(entity =>
        {
            entity.ToTable("library_item_status_history");

            entity.HasKey(x => x.Id).HasName("PK_library_item_status_history");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.LibraryItemId).HasColumnName("library_item_id");
            entity.Property(x => x.ChangedBy).HasColumnName("changed_by");
            entity.Property(x => x.OldStatus).HasColumnName("old_status").HasMaxLength(20).HasConversion<string>();
            entity.Property(x => x.NewStatus).HasColumnName("new_status").HasMaxLength(20).HasConversion<string>();
            entity.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => new { x.LibraryItemId, x.CreatedAt }).HasDatabaseName("IX_library_item_status_history_item_created_at");

            entity.HasOne(x => x.LibraryItem).WithMany(x => x.StatusHistories).HasForeignKey(x => x.LibraryItemId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_library_item_status_history_item");
            entity.HasOne(x => x.ChangedByUser).WithMany(x => x.LibraryItemStatusHistories).HasForeignKey(x => x.ChangedBy).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_library_item_status_history_user");
        });
    }

    private static void ConfigureUploadSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UploadSession>(entity =>
        {
            entity.ToTable("upload_sessions");

            entity.HasKey(x => x.Id).HasName("PK_upload_sessions");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.CardRegistrationId).HasColumnName("card_registration_id");
            entity.Property(x => x.ObjectKey).HasColumnName("object_key").HasMaxLength(500);
            entity.Property(x => x.UploadType).HasColumnName("upload_type").HasMaxLength(20).HasConversion<string>();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().HasDefaultValue(UploadSessionStatus.PENDING);
            entity.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("datetime2(7)");
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_upload_sessions_user_id");
            entity.HasIndex(x => new { x.Status, x.ExpiresAt }).HasDatabaseName("IX_upload_sessions_status_expired_at");

            entity.HasOne(x => x.User).WithMany(x => x.UploadSessions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_upload_sessions_user");
            entity.HasOne(x => x.CardRegistration).WithMany().HasForeignKey(x => x.CardRegistrationId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_upload_sessions_card_registration");

            entity.ToTable(t => { t.HasCheckConstraint("CK_upload_sessions_status", "[status] IN ('PENDING','COMPLETED','EXPIRED','FAILED')"); });
        });
    }

    private static void ConfigureRefreshToken(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");

            entity.HasKey(x => x.Id)
                .HasName("PK_refresh_tokens");

            entity.Property(x => x.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(x => x.TokenHash)
                .HasColumnName("token_hash")
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(x => x.FamilyId)
                .HasColumnName("family_id")
                .IsRequired();

            entity.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            entity.Property(x => x.RevokedAt)
                .HasColumnName("revoked_at")
                .HasColumnType("datetime2(7)");

            entity.Property(x => x.ReplacedBy)
                .HasColumnName("replaced_by");

            entity.HasIndex(x => x.TokenHash)
                .IsUnique()
                .HasDatabaseName("UQ_refresh_tokens_token_hash");

            entity.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_refresh_tokens_user_id");

            entity.HasIndex(x => x.FamilyId)
                .HasDatabaseName("IX_refresh_tokens_family_id");

            entity.HasIndex(x => x.ExpiresAt)
                .HasDatabaseName("IX_refresh_tokens_expires_at");

            entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_refresh_tokens_user");

            entity.HasOne(x => x.ReplacedByToken)
                .WithMany()
                .HasForeignKey(x => x.ReplacedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_refresh_tokens_replaced_by");
        });
    }



    private static void ConfigureRevokedToken(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RevokedToken>(entity =>
        {
            entity.ToTable("revoked_tokens");

            entity.HasKey(x => x.Id).HasName("PK_revoked_tokens");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.Jti).HasColumnName("jti").IsRequired();
            entity.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("datetime2(7)");
            entity.Property(x => x.RevokedAt).HasColumnName("revoked_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Jti).IsUnique().HasDatabaseName("UQ_revoked_tokens_jti");
            entity.HasIndex(x => x.ExpiresAt).HasDatabaseName("IX_revoked_tokens_expires_at");

            entity.HasOne(x => x.User).WithMany(x => x.RevokedTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_revoked_tokens_user");
        });
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasKey(x => x.Id).HasName("PK_categories");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(x => x.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UQ_categories_name");
        });
    }

    private static void ConfigureResourceCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResourceCategory>(entity =>
        {
            entity.ToTable("resource_categories");

            entity.HasKey(x => new { x.ResourceId, x.CategoryId }).HasName("PK_resource_categories");

            entity.Property(x => x.ResourceId).HasColumnName("resource_id");
            entity.Property(x => x.CategoryId).HasColumnName("category_id");

            entity.HasIndex(x => x.CategoryId).HasDatabaseName("IX_resource_categories_category_id");

            entity.HasOne(x => x.Resource).WithMany(x => x.ResourceCategories).HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_resource_categories_resource");
            entity.HasOne(x => x.Category).WithMany(x => x.ResourceCategories).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_resource_categories_category");
        });
    }

    private static void ConfigureAuthor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");

            entity.HasKey(x => x.Id).HasName("PK_authors");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(x => x.Biography).HasColumnName("biography");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
        });
    }

    private static void ConfigureResourceAuthor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResourceAuthor>(entity =>
        {
            entity.ToTable("resource_authors");

            entity.HasKey(x => new { x.ResourceId, x.AuthorId }).HasName("PK_resource_authors");

            entity.Property(x => x.ResourceId).HasColumnName("resource_id");
            entity.Property(x => x.AuthorId).HasColumnName("author_id");

            entity.HasIndex(x => x.AuthorId).HasDatabaseName("IX_resource_authors_author_id");

            entity.HasOne(x => x.Resource).WithMany(x => x.ResourceAuthors).HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_resource_authors_resource");
            entity.HasOne(x => x.Author).WithMany(x => x.ResourceAuthors).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_resource_authors_author");
        });
    }

    private static void ConfigurePublisher(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("publishers");

            entity.HasKey(x => x.Id).HasName("PK_publishers");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
            entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30);
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UQ_publishers_name");
        });
    }

    private static void ConfigureLocation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("locations");

            entity.HasKey(x => x.Id).HasName("PK_locations");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UQ_locations_name");
        });
    }

    private static void ConfigureDocumentType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("document_types");

            entity.HasKey(x => x.Id).HasName("PK_document_types");
            entity.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UQ_document_types_name");
        });
    }
}