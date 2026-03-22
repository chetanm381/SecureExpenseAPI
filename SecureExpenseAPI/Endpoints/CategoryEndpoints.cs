using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;
using SecureExpenseAPI.DTOs.Categories;
using SecureExpenseAPI.Entities;
using SecureExpenseAPI.Utils;

namespace SecureExpenseAPI.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var categoryGroup = app.MapGroup("/categories").RequireAuthorization();

        categoryGroup.MapGet("/", async (ClaimsPrincipal user, AppDbContext dbContext) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var categories = await dbContext.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
            return Results.Ok(categories);
        });

        categoryGroup.MapPost("/", async (ClaimsPrincipal user, CreateCategoryRequest request, AppDbContext dbContext) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { message = "Category name is required" });
            }

            if (request.Name.Length > 100)
            {
                return Results.BadRequest(new { message = "Category name cannot exceed 100 characters" });
            }

            var userId = UserUtils.GetUserIdFromClaims(user);

            // Check if name already exists for this user
            var nameExists = await dbContext.Categories.AnyAsync(c => c.UserId == userId && c.Name.ToLower() == request.Name.ToLower());
            if (nameExists)
            {
                return Results.Conflict(new { message = $"Category with name '{request.Name}' already exists." });
            }
            
            var category = new Category
            {
                Name = request.Name,
                UserId = userId
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/categories/{category.Id}", new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            });
        });

        categoryGroup.MapPut("/{id}", async (ClaimsPrincipal user, int id, UpdateCategoryRequest request, AppDbContext dbContext) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { message = "Category name is required" });
            }

            if (request.Name.Length > 100)
            {
                return Results.BadRequest(new { message = "Category name cannot exceed 100 characters" });
            }

            var userId = UserUtils.GetUserIdFromClaims(user);

            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
            {
                return Results.NotFound();
            }

            // Check if name already exists for another category of the same user
            var nameExists = await dbContext.Categories
                .AnyAsync(c => c.UserId == userId && c.Id != id && c.Name.ToLower() == request.Name.ToLower());
                
            if (nameExists)
            {
                return Results.Conflict(new { message = $"Category with name '{request.Name}' already exists." });
            }

            category.Name = request.Name;
            await dbContext.SaveChangesAsync();

            return Results.Ok(new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            });
        });

        categoryGroup.MapDelete("/{id}", async (ClaimsPrincipal user, int id, AppDbContext dbContext) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
            {
                return Results.NotFound();
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
