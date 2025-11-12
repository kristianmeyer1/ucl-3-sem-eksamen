using Danplanner.Application.Interfaces;
using Danplanner.Persistence.DbMangagerDir;
using Danplanner.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string fra appsettings.json
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// Vælg provider der matcher din DB (MySQL vist her)
builder.Services.AddDbContext<DbManager>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddRazorPages();

var app = builder.Build();

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
