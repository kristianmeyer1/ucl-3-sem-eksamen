using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Interfaces.AdminInterfaces;
using Danplanner.Application.Interfaces.BookingInterfaces;
using Danplanner.Application.Interfaces.AuthInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Services;
using Danplanner.Domain.Interfaces;
using Danplanner.Infrastructure.Services;
using Danplanner.Persistence.DbMangagerDir;
using Danplanner.Persistence.Repositories;
using Danplanner.Persistence.Repositories.UserRepositories;
using Danplanner.Persistence.Repositories.AccommodationRepositories;
using Danplanner.Persistence.Repositories.AddonRepositories;
using Danplanner.Persistence.Repositories.AdminRepositories;
using Danplanner.Persistence.Repositories.BookingRepositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Database connection
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DbManager>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

// Add HttpContextAccessor for Razor pages and layout injection
builder.Services.AddHttpContextAccessor();

// Controllers & Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Translation services
builder.Services.AddSingleton<ITranslationService>(sp =>
    new GoogleTranslationService(builder.Configuration["GoogleCloud:danplanner"]));
builder.Services.AddScoped<ContentTranslationHandler>();

// Addon builders
builder.Services.AddHttpClient<AddonService>();
builder.Services.AddScoped<IAddonGetAll, AddonRepositoryGet>();

// Admin builders
builder.Services.AddScoped<IAdminAdd, AdminRepositoryPost>();
builder.Services.AddScoped<IAdminGetById, AdminRepositoryGet>();
builder.Services.AddScoped<IAdminGetAll, AdminRepositoryGet>();

// Booking builders
builder.Services.AddScoped<IBookingAdd, BookingRepositoryPost>();

// User builders
builder.Services.AddHttpClient<UserService>();
builder.Services.AddScoped<IUserAdd, UserRepositoryPost>();
builder.Services.AddScoped<IUserDelete, UserRepositoryDelete>();
builder.Services.AddScoped<IUserGetAll, UserRepositoryGet>();
builder.Services.AddScoped<IUserGetByEmail, UserRepositoryGet>();
builder.Services.AddScoped<IUserGetById, UserRepositoryGet>();
builder.Services.AddScoped<IUserUpdate, UserRepositoryPut>();

// Authentication builders
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Accommodation builders
builder.Services.AddScoped<IAccommodationTransfer, AccommodationService>();
builder.Services.AddScoped<IAccommodationGetAll, AccommodationRepositoryGet>();
builder.Services.AddScoped<IAccommodationGetById, AccommodationRepositoryGet>();
builder.Services.AddScoped<IAccommodationUpdate, AccommodationRepositoryPut>();

// HttpClient 
builder.Services.AddHttpClient();

// Cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// Build app
var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
