using Danplanner.Application.Interfaces;
using Danplanner.Application.Services;
using Danplanner.Persistence.DbMangagerDir;
using Danplanner.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string fra appsettings.json
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// Vælg provider der matcher din DB (MySQL vist her)
builder.Services.AddDbContext<DbManager>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();


var app = builder.Build();
app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
