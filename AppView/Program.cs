using AppApi.IService;
using AppApi.Service;
using AppData.Models;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Repository;
using AppView.Areas.Auth.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cho phép sử dụng các Razor Pages của Identity (nếu bạn dùng Identity UI)
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
builder.Logging.AddConsole(); // 👈 Ghi log ra terminal/console
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddHttpClient<IAuthRepository, AuthRepository>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});
builder.Services.AddScoped<ISanPhamRepo, SanPhamRepo>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromDays(2); // 24 giờ
});

builder.Services.AddScoped<ICoAoRepo, CoAoRepo>();
builder.Services.AddScoped<IMauSacRepo, MauSacRepo>();
builder.Services.AddScoped<ISizeRepo, SizeRepo>();
builder.Services.AddScoped<ISanPhamRepo, SanPhamRepo>();
builder.Services.AddScoped<ITaAoRepo, TaAoRepo>();
builder.Services.AddScoped<ISanPhamCTRepo, SanPhamCTRepo>();
builder.Services.AddHttpClient<IDanhMucRePo, DanhMucRepo>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddDistributedMemoryCache();

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