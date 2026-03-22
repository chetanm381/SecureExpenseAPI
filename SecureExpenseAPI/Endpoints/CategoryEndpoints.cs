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
            var userId = UserUtils.GetUserIdFromClaims(user);

            var validationResult = await CategoryValidationUtils.ValidateCategoryAsync(request.Name, userId, dbContext);
            if (!validationResult.IsValid)
            {
                // Uniqueness failure (already exists) should return 409 Conflict
                if (validationResult.ErrorMessage?.Contains("already exists") == true)
                {
                    return Results.Conflict(new { message = validationResult.ErrorMessage });
                }
                return Results.BadRequest(new { message = validationResult.ErrorMessage });
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
            var userId = UserUtils.GetUserIdFromClaims(user);

            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
            {
                return Results.NotFound();
            }

            var validationResult = await CategoryValidationUtils.ValidateCategoryAsync(request.Name, userId, dbContext, id);
            if (!validationResult.IsValid)
            {
                if (validationResult.ErrorMessage?.Contains("already exists") == true)
                {
                    return Results.Conflict(new { message = validationResult.ErrorMessage });
                }
                return Results.BadRequest(new { message = validationResult.ErrorMessage });
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
