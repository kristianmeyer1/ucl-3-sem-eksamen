using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);
// Add a CORS policy so the gateway responds with the proper headers
// when the Blazor client (https://localhost:7091) calls the proxy at http://localhost:8080
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
		policy.WithOrigins("https://localhost:7091")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
	);
});

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.UseCors();

app.MapGet("/health", () => Results.Ok("OK"));

// Ensure the reverse proxy is mapped after CORS middleware so preflight
// OPTIONS requests are handled and responses include CORS headers.
app.MapReverseProxy();

app.Run();
