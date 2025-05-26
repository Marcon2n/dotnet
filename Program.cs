var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // Register Controllers
builder.Services.AddSingleton<UserRepository>(); // Register your UserRepository

var app = builder.Build();

// Comment out HTTPS redirection for now
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();
