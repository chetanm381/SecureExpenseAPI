using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();




app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
