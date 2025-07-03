using AppApi.IService;
using AppApi.Service;
using Microsoft.EntityFrameworkCore;
using AppData.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ISizeService , SizeService>();
builder.Services.AddScoped<IMauSacService, MauSacService>();
builder.Services.AddScoped<ICoAoService, CoAoService>();
builder.Services.AddScoped<ITaAoService, TaAoService>();
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<ISanPhamCTService, SanPhamCTService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppApi v1");
    c.RoutePrefix = string.Empty; // Nếu muốn Swagger ở đường dẫn gốc
});
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
