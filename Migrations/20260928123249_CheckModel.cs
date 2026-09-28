using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteQuanLyThuVien.Migrations
{
    /// <inheritdoc />
    public partial class CheckModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    biography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    display_order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "document_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    notification_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "publishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "storage_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    content_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    file_size = table.Column<long>(type: "bigint", nullable: true),
                    object_key = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    isbn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    publication_year = table.Column<int>(type: "int", nullable: true),
                    language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    page_count = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    publisher_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    document_type_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resources", x => x.Id);
                    table.CheckConstraint("CK_resources_status", "[status] IN ('ACTIVE','INACTIVE')");
                    table.ForeignKey(
                        name: "FK_resources_document_type",
                        column: x => x.document_type_id,
                        principalTable: "document_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_resources_publisher",
                        column: x => x.publisher_id,
                        principalTable: "publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    citizen_id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    avatar_file_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "ACTIVE"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_avatar_file",
                        column: x => x.avatar_file_id,
                        principalTable: "storage_files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "digital_resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    resource_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    file_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    resource_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    access_level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "MEMBER"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_digital_resources", x => x.Id);
                    table.CheckConstraint("CK_digital_resources_access_level", "[access_level] IN ('PUBLIC','MEMBER','STAFF','HIDDEN')");
                    table.CheckConstraint("CK_digital_resources_status", "[status] IN ('ACTIVE','INACTIVE')");
                    table.CheckConstraint("CK_digital_resources_type", "[resource_type] IN ('EBOOK','DOCUMENT','AUDIOBOOK','OTHER')");
                    table.ForeignKey(
                        name: "FK_digital_resources_file",
                        column: x => x.file_id,
                        principalTable: "storage_files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_digital_resources_resource",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "library_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    resource_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    location_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "AVAILABLE"),
                    item_condition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "GOOD"),
                    acquired_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    acquisition_price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_library_items", x => x.Id);
                    table.CheckConstraint("CK_library_items_condition", "[item_condition] IN ('NEW','GOOD','WORN','DAMAGED')");
                    table.CheckConstraint("CK_library_items_status", "[status] IN ('AVAILABLE','RESERVED','BORROWED','LOST','DAMAGED','MAINTENANCE','REMOVED')");
                    table.ForeignKey(
                        name: "FK_library_items_location",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_library_items_resource",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "resource_authors",
                columns: table => new
                {
                    resource_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    author_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_authors", x => new { x.resource_id, x.author_id });
                    table.ForeignKey(
                        name: "FK_resource_authors_author",
                        column: x => x.author_id,
                        principalTable: "authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_authors_resource",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_categories",
                columns: table => new
                {
                    resource_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_categories", x => new { x.resource_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_resource_categories_category",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_categories_resource",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    resource_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    file_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_primary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resource_images_file",
                        column: x => x.file_id,
                        principalTable: "storage_files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_resource_images_resource",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "library_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    membership_status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "ACTIVE"),
                    registered_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    expired_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_library_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_library_members_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "notification_recipients",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    read_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_recipients", x => new { x.notification_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_notification_recipients_notification",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notification_recipients_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    token_hash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    family_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    revoked_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    replaced_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_replaced_by",
                        column: x => x.replaced_by,
                        principalTable: "refresh_tokens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_refresh_tokens_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "revoked_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    jti = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revoked_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_revoked_tokens_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_staff_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_roles_role",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "copy_issues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    library_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reported_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    issue_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "OPEN"),
                    resolved_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    resolved_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_copy_issues", x => x.Id);
                    table.CheckConstraint("CK_copy_issues_status", "[status] IN ('OPEN','IN_PROGRESS','RESOLVED','CANCELLED')");
                    table.ForeignKey(
                        name: "FK_copy_issues_library_item",
                        column: x => x.library_item_id,
                        principalTable: "library_items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_copy_issues_reported_by",
                        column: x => x.reported_by,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_copy_issues_resolved_by",
                        column: x => x.resolved_by,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "library_item_status_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    library_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    old_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    new_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    changed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_library_item_status_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_library_item_status_history_item",
                        column: x => x.library_item_id,
                        principalTable: "library_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_library_item_status_history_user",
                        column: x => x.changed_by,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "borrow_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    member_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    request_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "PENDING"),
                    processed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_borrow_requests", x => x.Id);
                    table.CheckConstraint("CK_borrow_requests_status", "[status] IN ('PENDING','APPROVED','READY_FOR_PICKUP','DELIVERING','COMPLETED','REJECTED','CANCELLED','EXPIRED')");
                    table.CheckConstraint("CK_borrow_requests_type", "[request_type] IN ('RESERVATION','DELIVERY')");
                    table.ForeignKey(
                        name: "FK_borrow_requests_member",
                        column: x => x.member_id,
                        principalTable: "library_members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_borrow_requests_processed_by",
                        column: x => x.processed_by,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "library_cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    member_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    card_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    issued_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    expired_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_library_cards", x => x.Id);
                    table.CheckConstraint("CK_library_cards_status", "[status] IN ('PENDING','ACTIVE','BLOCKED','EXPIRED','CANCELLED')");
                    table.ForeignKey(
                        name: "FK_library_cards_member",
                        column: x => x.member_id,
                        principalTable: "library_members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    member_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    transaction_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "ONLINE_PAYMENT"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    total_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    expired_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_transactions", x => x.Id);
                    table.CheckConstraint("CK_payment_transactions_amount", "[total_amount] >= 0");
                    table.CheckConstraint("CK_payment_transactions_method", "[payment_method] IN ('ONLINE_PAYMENT')");
                    table.CheckConstraint("CK_payment_transactions_status", "[status] IN ('PENDING','PROCESSING','PAID','FAILED','CANCELLED')");
                    table.ForeignKey(
                        name: "FK_payment_transactions_member",
                        column: x => x.member_id,
                        principalTable: "library_members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "borrow_request_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    borrow_request_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    resource_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    library_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "PENDING"),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_borrow_request_items", x => x.Id);
                    table.CheckConstraint("CK_borrow_request_items_quantity", "[quantity] > 0");
                    table.CheckConstraint("CK_borrow_request_items_status", "[status] IN ('PENDING','APPROVED','REJECTED','CANCELLED','READY')");
                    table.ForeignKey(
                        name: "FK_borrow_request_items_library_item",
                        column: x => x.library_item_id,
                        principalTable: "library_items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_borrow_request_items_request",
                        column: x => x.borrow_request_id,
                        principalTable: "borrow_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_borrow_request_items_resource",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    borrow_request_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    recipient_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    recipient_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "PENDING"),
                    delivered_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_deliveries_borrow_request",
                        column: x => x.borrow_request_id,
                        principalTable: "borrow_requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    member_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    borrow_request_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    borrowed_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    completed_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loans", x => x.Id);
                    table.CheckConstraint("CK_loans_status", "[status] IN ('ACTIVE','COMPLETED','CANCELLED')");
                    table.ForeignKey(
                        name: "FK_loans_borrow_request",
                        column: x => x.borrow_request_id,
                        principalTable: "borrow_requests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_loans_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_loans_member",
                        column: x => x.member_id,
                        principalTable: "library_members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "card_registrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    payment_transaction_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    id_document_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    id_document_front_file_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    id_document_back_file_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    avatar_file_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "WAITING_FOR_INFORMATION"),
                    registration_token_hash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    registration_token_expired_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    registration_token_revoked_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    reviewed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    rejection_reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_card_registrations", x => x.Id);
                    table.CheckConstraint("CK_card_registrations_status", "[status] IN ('WAITING_FOR_INFORMATION','PENDING','APPROVED','REJECTED','CANCELLED','EXPIRED')");
                    table.ForeignKey(
                        name: "FK_card_registrations_avatar_file",
                        column: x => x.avatar_file_id,
                        principalTable: "storage_files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_card_registrations_back_file",
                        column: x => x.id_document_back_file_id,
                        principalTable: "storage_files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_card_registrations_front_file",
                        column: x => x.id_document_front_file_id,
                        principalTable: "storage_files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_card_registrations_payment",
                        column: x => x.payment_transaction_id,
                        principalTable: "payment_transactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_card_registrations_reviewer",
                        column: x => x.reviewed_by,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_card_registrations_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "loan_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    loan_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    library_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    due_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    returned_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "BORROWED"),
                    condition_at_loan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    condition_at_return = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loan_items", x => x.Id);
                    table.CheckConstraint("CK_loan_items_status", "[status] IN ('BORROWED','OVERDUE','RETURNED','LOST')");
                    table.ForeignKey(
                        name: "FK_loan_items_library_item",
                        column: x => x.library_item_id,
                        principalTable: "library_items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_loan_items_loan",
                        column: x => x.loan_id,
                        principalTable: "loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "upload_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    card_registration_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ObjectKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadType = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    expires_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    completed_at = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_upload_sessions", x => x.Id);
                    table.CheckConstraint("CK_upload_sessions_status", "[status] IN ('PENDING','COMPLETED','EXPIRED','CANCELLED')");
                    table.ForeignKey(
                        name: "FK_upload_sessions_card_registration",
                        column: x => x.card_registration_id,
                        principalTable: "card_registrations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_upload_sessions_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "charges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    member_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    loan_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    charge_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "UNPAID"),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_charges", x => x.Id);
                    table.CheckConstraint("CK_charges_amount", "[amount] >= 0");
                    table.CheckConstraint("CK_charges_status", "[status] IN ('UNPAID','PAID','WAIVED')");
                    table.CheckConstraint("CK_charges_type", "[charge_type] IN ('CARD_FEE','OVERDUE','DAMAGED','LOST')");
                    table.ForeignKey(
                        name: "FK_charges_loan_item",
                        column: x => x.loan_item_id,
                        principalTable: "loan_items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_charges_member",
                        column: x => x.member_id,
                        principalTable: "library_members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "loan_renewals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    loan_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    old_due_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    new_due_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    renewed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loan_renewals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loan_renewals_loan_item",
                        column: x => x.loan_item_id,
                        principalTable: "loan_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_loan_renewals_renewed_by",
                        column: x => x.renewed_by,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "payment_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    payment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    charge_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_items", x => x.Id);
                    table.CheckConstraint("CK_payment_items_amount", "[amount] >= 0");
                    table.ForeignKey(
                        name: "FK_payment_items_charge",
                        column: x => x.charge_id,
                        principalTable: "charges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_payment_items_payment",
                        column: x => x.payment_id,
                        principalTable: "payment_transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_borrow_request_items_library_item_id",
                table: "borrow_request_items",
                column: "library_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_request_items_request_id",
                table: "borrow_request_items",
                column: "borrow_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_request_items_resource_id",
                table: "borrow_request_items",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_requests_member_id",
                table: "borrow_requests",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_requests_processed_by",
                table: "borrow_requests",
                column: "processed_by");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_requests_status",
                table: "borrow_requests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_card_registrations_avatar_file_id",
                table: "card_registrations",
                column: "avatar_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_card_registrations_id_document_back_file_id",
                table: "card_registrations",
                column: "id_document_back_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_card_registrations_id_document_front_file_id",
                table: "card_registrations",
                column: "id_document_front_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_card_registrations_reviewed_by",
                table: "card_registrations",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "IX_card_registrations_status",
                table: "card_registrations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_card_registrations_user_id",
                table: "card_registrations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_card_registrations_payment",
                table: "card_registrations",
                column: "payment_transaction_id",
                unique: true,
                filter: "[payment_transaction_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_card_registrations_token",
                table: "card_registrations",
                column: "registration_token_hash",
                unique: true,
                filter: "[registration_token_hash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_charges_loan_item_id",
                table: "charges",
                column: "loan_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_charges_member_id",
                table: "charges",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_charges_status",
                table: "charges",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_copy_issues_library_item_id",
                table: "copy_issues",
                column: "library_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_copy_issues_reported_by",
                table: "copy_issues",
                column: "reported_by");

            migrationBuilder.CreateIndex(
                name: "IX_copy_issues_resolved_by",
                table: "copy_issues",
                column: "resolved_by");

            migrationBuilder.CreateIndex(
                name: "IX_copy_issues_status",
                table: "copy_issues",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "UQ_deliveries_borrow_request",
                table: "deliveries",
                column: "borrow_request_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_digital_resources_file_id",
                table: "digital_resources",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_digital_resources_resource_id",
                table: "digital_resources",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_digital_resources_resource_type",
                table: "digital_resources",
                column: "resource_type");

            migrationBuilder.CreateIndex(
                name: "UQ_document_types_name",
                table: "document_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_library_cards_member",
                table: "library_cards",
                column: "member_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_library_cards_number",
                table: "library_cards",
                column: "card_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_library_item_status_history_changed_by",
                table: "library_item_status_history",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "IX_library_item_status_history_item_created_at",
                table: "library_item_status_history",
                columns: new[] { "library_item_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_library_items_location_id",
                table: "library_items",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_library_items_resource_id",
                table: "library_items",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_library_items_resource_status",
                table: "library_items",
                columns: new[] { "resource_id", "status" });

            migrationBuilder.CreateIndex(
                name: "UQ_library_items_barcode",
                table: "library_items",
                column: "barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_library_members_user_id",
                table: "library_members",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loan_items_due_at",
                table: "loan_items",
                column: "due_at");

            migrationBuilder.CreateIndex(
                name: "IX_loan_items_library_item_id",
                table: "loan_items",
                column: "library_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_loan_items_loan_id",
                table: "loan_items",
                column: "loan_id");

            migrationBuilder.CreateIndex(
                name: "IX_loan_items_status",
                table: "loan_items",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_loan_renewals_loan_item_id",
                table: "loan_renewals",
                column: "loan_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_loan_renewals_renewed_by",
                table: "loan_renewals",
                column: "renewed_by");

            migrationBuilder.CreateIndex(
                name: "IX_loans_borrow_request_id",
                table: "loans",
                column: "borrow_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_loans_created_by",
                table: "loans",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_loans_member_id",
                table: "loans",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_loans_status",
                table: "loans",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "UQ_locations_name",
                table: "locations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_recipients_user_id",
                table: "notification_recipients",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_recipients_user_unread",
                table: "notification_recipients",
                columns: new[] { "user_id", "is_read" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_items_charge_id",
                table: "payment_items",
                column: "charge_id");

            migrationBuilder.CreateIndex(
                name: "UQ_payment_items_payment_charge",
                table: "payment_items",
                columns: new[] { "payment_id", "charge_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_member_id",
                table: "payment_transactions",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_status",
                table: "payment_transactions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "UQ_payment_transactions_code",
                table: "payment_transactions",
                column: "transaction_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_publishers_name",
                table: "publishers",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_expires_at",
                table: "refresh_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_family_id",
                table: "refresh_tokens",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_replaced_by",
                table: "refresh_tokens",
                column: "replaced_by");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resource_authors_author_id",
                table: "resource_authors",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_categories_category_id",
                table: "resource_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_images_file_id",
                table: "resource_images",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "UX_resource_images_primary",
                table: "resource_images",
                column: "resource_id",
                unique: true,
                filter: "[is_primary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_resources_document_type_id",
                table: "resources",
                column: "document_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_resources_publisher_id",
                table: "resources",
                column: "publisher_id");

            migrationBuilder.CreateIndex(
                name: "IX_resources_status",
                table: "resources",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_resources_title",
                table: "resources",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "UQ_resources_isbn",
                table: "resources",
                column: "isbn",
                unique: true,
                filter: "[isbn] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_revoked_tokens_expires_at",
                table: "revoked_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_revoked_tokens_user_id",
                table: "revoked_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_revoked_tokens_jti",
                table: "revoked_tokens",
                column: "jti",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_staff_employee_code",
                table: "staff",
                column: "employee_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_staff_user_id",
                table: "staff",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_storage_files_object_key",
                table: "storage_files",
                column: "object_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_upload_sessions_card_registration_id",
                table: "upload_sessions",
                column: "card_registration_id");

            migrationBuilder.CreateIndex(
                name: "IX_upload_sessions_status_expired_at",
                table: "upload_sessions",
                columns: new[] { "status", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_upload_sessions_user_id",
                table: "upload_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_avatar_file_id",
                table: "users",
                column: "avatar_file_id",
                unique: true,
                filter: "[avatar_file_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_users_citizen_id",
                table: "users",
                column: "citizen_id",
                unique: true,
                filter: "[citizen_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "borrow_request_items");

            migrationBuilder.DropTable(
                name: "copy_issues");

            migrationBuilder.DropTable(
                name: "deliveries");

            migrationBuilder.DropTable(
                name: "digital_resources");

            migrationBuilder.DropTable(
                name: "library_cards");

            migrationBuilder.DropTable(
                name: "library_item_status_history");

            migrationBuilder.DropTable(
                name: "loan_renewals");

            migrationBuilder.DropTable(
                name: "notification_recipients");

            migrationBuilder.DropTable(
                name: "payment_items");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "resource_authors");

            migrationBuilder.DropTable(
                name: "resource_categories");

            migrationBuilder.DropTable(
                name: "resource_images");

            migrationBuilder.DropTable(
                name: "revoked_tokens");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "upload_sessions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "charges");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "card_registrations");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "loan_items");

            migrationBuilder.DropTable(
                name: "payment_transactions");

            migrationBuilder.DropTable(
                name: "library_items");

            migrationBuilder.DropTable(
                name: "loans");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "borrow_requests");

            migrationBuilder.DropTable(
                name: "document_types");

            migrationBuilder.DropTable(
                name: "publishers");

            migrationBuilder.DropTable(
                name: "library_members");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "storage_files");
        }
    }
}
