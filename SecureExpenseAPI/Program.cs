using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;
using SecureExpenseAPI.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the custom password hasher built on top of ASP.NET Core Identity
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();




app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
