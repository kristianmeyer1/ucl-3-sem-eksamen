using Danplanner.Application.Interfaces;
using Danplanner.Application.Services;
using Danplanner.Persistence.DbMangagerDir;
using Danplanner.Persistence.Repositories;
using Danplanner.Domain.Interfaces;
using Danplanner.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Danplanner.Application.Interfaces.AdminInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Interfaces.AccommodationInterfaces;


var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DbManager>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<ITranslationService>(sp =>
    new GoogleTranslationService(builder.Configuration["GoogleCloud:danplanner"]));
builder.Services.AddScoped<ContentTranslationHandler>();

// Addon builders
builder.Services.AddHttpClient<IAddonGetAll, AddonService>();
builder.Services.AddScoped<IAddonGetAll, AddonRepository>();

// Admin builders
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

// Calender builders

// Log builders

// Map Builders

// Packages builders

// Seasons builders

// User builders
builder.Services.AddHttpClient<UserService>();
builder.Services.AddScoped<IUserGetAll, UserRepository>();
builder.Services.AddScoped<IUserGetById, UserRepository>();
//builder.Services.AddHttpClient<IUserRepository, UserService>();

// Authentication builders
builder.Services.AddScoped<IAuthService, AuthService>();

// Accommodation builders
builder.Services.AddScoped<IAccommodationService, AccommodationService>();
builder.Services.AddScoped<IAccommodationRepository, AccommodationRepository>();
builder.Services.AddScoped<IAccommodationAvailabilityRepository, AccommodationAvailabilityRepository>();

// HttpClient for general use
builder.Services.AddHttpClient();

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
app.MapControllers();

app.Run();
