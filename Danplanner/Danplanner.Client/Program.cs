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

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IUserRepository, UserService>();
builder.Services.AddHttpClient<IAddonRepository, AddonService>();
builder.Services.AddHttpClient<UserService>();
builder.Services.AddScoped<IAccommodationService, AccommodationService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

// HttpClient 
builder.Services.AddScoped<IAddonRepository, AddonRepository>();
builder.Services.AddScoped<IAccommodationRepository, AccommodationRepository>();
builder.Services.AddScoped<IAccommodationAvailabilityRepository, AccommodationAvailabilityRepository>();
builder.Services.AddHttpClient();

// Cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login";
    });
builder.Services.AddAuthorization();

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
