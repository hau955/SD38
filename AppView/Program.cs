using AppApi.IService;
using AppApi.Service;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Repository;
using AppView.Areas.Auth.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
builder.Services.AddHttpClient<ISanPhamRepo, SanPhamRepo>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7221/");
});
builder.Services.AddHttpClient<IAuthRepository, AuthRepository>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7221/");
});

// Cấu hình HttpClient cho từng repo gọi API

builder.Services.AddScoped<IMauSacRepo, MauSacRepo>();
builder.Services.AddScoped<ISizeRepo, SizeRepo>();
builder.Services.AddScoped<ISanPhamRepo, SanPhamRepo>();

builder.Services.AddScoped<ISanPhamCTRepo, SanPhamCTRepo>();
builder.Services.AddScoped<IChatLieuRepo, ChatLieuRepo>();
builder.Services.AddHttpClient<IDanhMucRePo, DanhMucRepo>();
builder.Services.AddScoped<IProfileRepo, ProfileRepo>();

// Thêm dịch vụ cần thiết cho session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    // Cấu hình các tùy chọn cho session
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session tồn tại
    options.Cookie.HttpOnly = true; // Đảm bảo cookie chỉ được truy cập bởi máy chủ
    options.Cookie.IsEssential = true; // Đánh dấu cookie là cần thiết cho ứng dụng
});
// (Không cần dòng AddScoped nữa!)

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

app.UseAuthorization();

// Routing mặc định
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
     name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
