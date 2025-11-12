using Danplanner.Application.Interfaces;
using Danplanner.Application.Services;
using Danplanner.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Danplanner.Persistence.Repositories;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAdminRepository,AdminRepository>();
builder.Services.AddScoped<AdminService>();

await builder.Build().RunAsync();
