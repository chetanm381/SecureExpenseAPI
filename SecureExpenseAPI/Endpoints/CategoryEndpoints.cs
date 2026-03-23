using System.Security.Claims;
using SecureExpenseAPI.DTOs.Categories;
using SecureExpenseAPI.Services.Categories;
using SecureExpenseAPI.Utils;

namespace SecureExpenseAPI.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var categoryGroup = app.MapGroup("/categories").RequireAuthorization();

        categoryGroup.MapGet("/", async (ClaimsPrincipal user, ICategoryService categoryService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var categories = await categoryService.GetCategoriesAsync(userId);
            return Results.Ok(categories);
        });

        categoryGroup.MapPost("/", async (ClaimsPrincipal user, CreateCategoryRequest request, ICategoryService categoryService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            
            var result = await categoryService.CreateCategoryAsync(userId, request);
            if (result.ErrorMessage != null)
            {
                if (result.IsConflict)
                {
                    return Results.Conflict(new { message = result.ErrorMessage });
                }
                return Results.BadRequest(new { message = result.ErrorMessage });
            }

            return Results.Created($"/categories/{result.Data!.Id}", result.Data);
        });

        categoryGroup.MapPut("/{id}", async (ClaimsPrincipal user, int id, UpdateCategoryRequest request, ICategoryService categoryService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            
            var result = await categoryService.UpdateCategoryAsync(userId, id, request);

            if (result.IsNotFound)
            {
                return Results.NotFound();
            }

            if (result.ErrorMessage != null)
            {
                if (result.IsConflict)
                {
                    return Results.Conflict(new { message = result.ErrorMessage });
                }
                return Results.BadRequest(new { message = result.ErrorMessage });
            }

            return Results.Ok(result.Data);
        });

        categoryGroup.MapDelete("/{id}", async (ClaimsPrincipal user, int id, ICategoryService categoryService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var isDeleted = await categoryService.DeleteCategoryAsync(userId, id);

            if (!isDeleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });
    }
}
