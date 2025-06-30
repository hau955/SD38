using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISanPhamRepo,SanPhamRepo>();

// Cấu hình HttpClient cho từng repo gọi API
builder.Services.AddScoped<IMauSacRepo, MauSacRepo>();

builder.Services.AddScoped<ISizeRepo, SizeRepo>();

builder.Services.AddScoped<ICoAoRepo, CoAoRepo>();

builder.Services.AddScoped<ITaAoRepo, TaAoRepo>();
builder.Services.AddScoped<ISanPhamCTRepo, SanPhamCTRepo>();


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


app.UseRouting();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
