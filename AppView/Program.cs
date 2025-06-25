using AppView.Areas.Admin.Repository;
using AppView.Areas.Auth.Repository;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
// Đăng ký Session
builder.Services.AddSession();

// Đăng ký HttpClient
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
builder.Services.AddHttpClient<ISanPhamRepo, SanPhamRepo>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IAuthRepository, AuthRepository>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// CORS
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

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
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
    pattern: "{area=Auth}/{controller=Auth}/{action=Login}/{id?}");

app.Run();
