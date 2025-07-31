using CasaApp.Api.Data;
using CasaApp.Api.Repositories;
using CasaApp.Api.Services;
using Microsoft.EntityFrameworkCore;

#region Builder

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"ENV: {builder.Environment.EnvironmentName}");

#endregion

#region Configuration

// 🔌 Leer cadena de conexión desde appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#endregion

#region Services

// 📦 Inyección de dependencias: DbContext
builder.Services.AddDbContext<CasaDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 📦 Inyección de dependencias: Repositorios y Servicios
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();

// 📦 AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// 📦 Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CasaApp API", Version = "v1" });
    c.EnableAnnotations();
});

#endregion

#region App

var app = builder.Build();

// 🌐 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

#endregion