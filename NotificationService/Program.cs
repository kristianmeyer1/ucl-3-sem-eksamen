//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/notify", (Notification notification) =>
{
    Console.WriteLine($"[Notification] {notification.Message}");
    return Results.Ok();
});

app.MapGet("/health", () => Results.Ok("OK"));

app.Run();

public record Notification(string Message);

