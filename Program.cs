using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeDetective;
using Minio;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Models;
using WebsiteQuanLyThuVien.Models.Authentication;
using WebsiteQuanLyThuVien.Repositories;
using WebsiteQuanLyThuVien.Services.Authentication;
using WebsiteQuanLyThuVien.Services.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();




// Đăng ký dịch vụ băm mật khẩu của Microsoft cho Class User 
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();


//Đăng ký services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Ra lệnh cho hệ thống: Nếu chưa đăng nhập mà vào trang không được phép, hãy trả về đây
    options.LoginPath = "/Account/Login";
});

//Đăng ký Repository

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

//Đăng ký jwt
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));


builder.Services
    /// <summary>
    /// Cấu hình dịch vụ Xác thực (Authentication) sử dụng cơ chế JWT Bearer làm phương thức mặc định.
    /// </summary>
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    /// <summary>
    /// Thêm và cấu hình các tùy chọn cho Middleware xử lý JWT Bearer.
    /// </summary>
    .AddJwtBearer(options =>
    {
        /// <summary>
        /// Lấy thông tin cấu hình JWT từ file cấu hình (appsettings.json hoặc appsettings.Development.json) và ánh xạ vào đối tượng JwtOptions.
        /// Sử dụng dấu chấm than (!) để khẳng định với Compiler rằng dữ liệu cấu hình này chắc chắn không bị null.
        /// </summary>
        var jwt = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtOptions>()!;

        /// <summary>
        /// Thiết lập các tham số dùng để kiểm tra và xác thực tính hợp lệ của chuỗi JWT gửi lên từ Client.
        /// </summary>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            /// <summary>
            /// Kích hoạt kiểm tra Nhà phát hành Token (Issuer).
            /// </summary>
            ValidateIssuer = true,
            /// <summary>
            /// Giá trị Nhà phát hành hợp lệ được chỉ định từ file cấu hình hệ thống.
            /// </summary>
            ValidIssuer = jwt.Issuer,

            /// <summary>
            /// Kích hoạt kiểm tra Đối tượng sử dụng Token (Audience).
            /// </summary>
            ValidateAudience = true,
            /// <summary>
            /// Giá trị Đối tượng sử dụng hợp lệ được chỉ định từ file cấu hình hệ thống.
            /// </summary>
            ValidAudience = jwt.Audience,

            /// <summary>
            /// Kích hoạt kiểm tra Thời hạn sử dụng (Lifetime) của Token nhằm đảm bảo Token chưa bị hết hạn.
            /// </summary>
            ValidateLifetime = true,

            /// <summary>
            /// Kích hoạt kiểm tra Chữ ký điện tử của Token (Issuer Signing Key) để chống giả mạo dữ liệu.
            /// </summary>
            ValidateIssuerSigningKey = true,
            /// <summary>
            /// Khóa đối xứng dùng để giải mã và xác thực chữ ký, được tạo ra từ chuỗi Secret Key bảo mật.
            /// </summary>
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)
            ),

            /// <summary>
            /// Đặt độ lệch thời gian cho phép giữa Server và Client về mức 0 giây.
            /// Loại bỏ hoàn toàn khoảng thời gian gia hạn mặc định (thường là 5 phút) của .NET để tăng tối đa tính chính xác của thời gian hết hạn.
            /// </summary>
            ClockSkew = TimeSpan.Zero
        };

        /// <summary>
        /// Cấu hình các sự kiện (Events) can thiệp vào vòng đời xử lý và xác thực chuỗi JWT.
        /// </summary>
        options.Events = new JwtBearerEvents
        {
            /// <summary>
            /// Sự kiện kích hoạt khi Middleware bắt đầu nhận và tìm kiếm Token từ Request gửi lên.
            /// </summary>
            /// <param name="context">Ngữ cảnh chứa thông tin Request của sự kiện nhận thông điệp.</param>
            /// <returns>Một tác vụ hoàn thành đồng bộ (Task.CompletedTask).</returns>
            OnMessageReceived = context =>
            {
                /// <summary>
                /// Thay đổi nơi lấy Token: Đọc trực tiếp chuỗi JWT từ Cookie có tên "access_token" của trình duyệt,
                /// thay vì tìm kiếm ở Header Authorization mặc định.
                /// </summary>
                context.Token =
                    context.Request.Cookies["access_token"];

                return Task.CompletedTask;
            },

            /// <summary>
            /// Sự kiện kích hoạt ngay sau khi Middleware đã giải mã toán học thành công, chữ ký đúng và Token còn hạn.
            /// Tiến hành kiểm tra lớp bảo mật thứ hai: Danh sách đen (Token bị hủy/đăng xuất).
            /// </summary>
            /// <param name="context">Ngữ cảnh chứa thông tin về Token và danh sách quyền (Principal) sau khi xác thực thành công.</param>
            /// <returns>Một tiến trình bất đồng bộ xử lý việc tra cứu dữ liệu.</returns>
            OnTokenValidated = async context =>
            {
                /// <summary>
                /// Sử dụng Service Locator Pattern để lấy ra dịch vụ ITokenRepository từ DI Container của HttpContext.
                /// </summary>
                var tokenRepository = context.HttpContext.RequestServices.GetRequiredService<ITokenRepository>();

                /// <summary>
                /// Tìm kiếm và trích xuất giá trị Claim mang tên mã định danh duy nhất của Token (Jti) bên trong JWT.
                /// </summary>
                var jtiValue = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                /// <summary>
                /// Thử chuyển đổi (Parse) chuỗi Jti vừa lấy được sang kiểu dữ liệu cấu trúc Guid.
                /// Nếu chuỗi không đúng định dạng Guid, đánh dấu xác thực thất bại và chặn đứng Request.
                /// </summary>
                if (!Guid.TryParse(jtiValue, out var jti))
                {
                    context.Fail("Invalid token.");
                    return;
                }

                /// <summary>
                /// Truy vấn trực tiếp xuống Database thông qua Repository để kiểm tra mã Jti này đã bị hủy bỏ hay chưa.
                /// </summary>
                var revoked = await tokenRepository.IsRevokedAsync(jti);

                /// <summary>
                /// Nếu mã Jti này đã nằm trong bảng danh sách đen (User đã bấm đăng xuất trước đó),
                /// gọi hàm Fail để từ chối quyền truy cập và trả về mã lỗi HTTP 401 Unauthorized.
                /// </summary>
                if (revoked)
                {
                    context.Fail("Token has been revoked.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


// SqlClient
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();


//Minio
builder.Services.Configure<MinioOptions>(
    builder.Configuration.GetSection("Minio"));

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var options =
        sp.GetRequiredService<IOptions<MinioOptions>>().Value;

    var client = new MinioClient()
        .WithEndpoint(options.Endpoint)
        .WithCredentials(
            options.AccessKey,
            options.SecretKey);

    if (options.UseSSL)
    {
        client = client.WithSSL();
    }

    return client.Build();
});

builder.Services.AddScoped<
    IFileStorageService,
    MinioFileStorageService>();


//Minetype
var inspector = new MimeDetective.ContentInspectorBuilder()
{
    Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
}.Build();


builder.Services.AddSingleton<IContentInspector>(inspector);

//Exception
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();

app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    //Code tạo password hash để test
    // var hasher = new PasswordHasher<User>();

    // var user = new User();

    // var hash = hasher.HashPassword(user, "Admin");

    // Console.WriteLine(hash);

    var storageService =
        scope.ServiceProvider
            .GetRequiredService<IFileStorageService>();

    await storageService.EnsureBucketExistsAsync();
}


app.Run();
