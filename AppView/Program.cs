using AppData.Models;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Repository;
using AppView.Areas.Auth.Repository;
using AppView.Areas.BanHangTaiQuay.IRepo;
using AppView.Areas.BanHangTaiQuay.Repository;
using AppView.Areas.OrderManagerment.Repositories;
using AppView.Clients;
using AppView.Clients.ApiClients;
using AppView.Helper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();
var isDev = builder.Environment.IsDevelopment();
var apiBaseUrl = isDev
    ? "https://localhost:7221/"
    : "https://your-production-api.com/";

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddHttpClient<IAuthRepository, AuthRepository>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromDays(2); // 24 giờ
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Auth/Login"; // 👈 Chính xác theo route của bạn
    options.AccessDeniedPath = "/Auth/Auth/AccessDenied"; // Nếu có trang từ chối truy cập
});

// Cấu hình HttpClient cho từng repo gọi API
// Thêm vào ConfigureServices/AddServices
builder.Services.AddAutoMapper(typeof(OrderMappingProfile).Assembly);
builder.Services.AddScoped<IMauSacRepo, MauSacRepo>();
builder.Services.AddScoped<ISizeRepo, SizeRepo>();
builder.Services.AddScoped<ISanPhamRepo, SanPhamRepo>();
builder.Services.AddScoped<IGioHangChiTietService, GioHangChiTietService>();

builder.Services.AddScoped<ISanPhamCTRepo, SanPhamCTRepo>();
builder.Services.AddScoped<IGiamGiaRepo, GiamGiaRepo>();
builder.Services.AddScoped<IBanHangfRepo, BanHangRepo>();
builder.Services.AddScoped<IThongKeRepo, ThongKeRepo>();
builder.Services.AddScoped<IChatLieuRepo, ChatLieuRepo>();
builder.Services.AddHttpClient<IDanhMucRePo, DanhMucRepo>();
builder.Services.AddScoped<IProfileRepo, ProfileRepo>();
builder.Services.AddHttpClient<IOrderManagementRepo, OrderManagementRepo>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddHttpClient<IEmployeeManagementRepo, EmployeeManagementRepo>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2097152;
});
builder.Services.AddSession(options =>
{
    // Cấu hình các tùy chọn cho session
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session tồn tại
    options.Cookie.HttpOnly = true; // Đảm bảo cookie chỉ được truy cập bởi máy chủ
    options.Cookie.IsEssential = true; // Đánh dấu cookie là cần thiết cho ứng dụng
});

// CORS (nếu có gọi từ web domain khác)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Hữu ích cho debug
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.Use(async (context, next) =>
{
    // Đảm bảo API luôn trả về JSON
    if (context.Request.Path.StartsWithSegments("/OrderManagerment") &&
        context.Request.Headers["Accept"].Contains("application/json"))
    {
        context.Response.OnStarting(() =>
        {
            context.Response.ContentType = "application/json";
            return Task.CompletedTask;
        });
    }
    await next();
});
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Routing mặc định
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
     name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();