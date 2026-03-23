using System.Security.Claims;
using SecureExpenseAPI.DTOs.Expenses;
using SecureExpenseAPI.Services.Expenses;
using SecureExpenseAPI.Utils;

namespace SecureExpenseAPI.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this WebApplication app)
    {
        var expenseGroup = app.MapGroup("/expenses").RequireAuthorization();

        expenseGroup.MapGet("/", async (ClaimsPrincipal user, IExpenseService expenseService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var expenses = await expenseService.GetExpensesAsync(userId);
            return Results.Ok(expenses);
        });

        expenseGroup.MapPost("/", async (ClaimsPrincipal user, CreateExpenseRequest request, IExpenseService expenseService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            
            var result = await expenseService.CreateExpenseAsync(userId, request);
            if (result.ErrorMessage != null)
            {
                return Results.BadRequest(new { message = result.ErrorMessage });
            }

            return Results.Created($"/expenses/{result.Data!.Id}", result.Data);
        });

        expenseGroup.MapGet("/{id}", async (ClaimsPrincipal user, int id, IExpenseService expenseService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var expense = await expenseService.GetExpenseAsync(userId, id);
                
            if (expense == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(expense);
        });

        expenseGroup.MapPut("/{id}", async (ClaimsPrincipal user, int id, UpdateExpenseRequest request, IExpenseService expenseService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var result = await expenseService.UpdateExpenseAsync(userId, id, request);

            if (result.ErrorMessage == "Expense not found.")
            {
                return Results.NotFound();
            }
            
            if (result.ErrorMessage != null)
            {
                return Results.BadRequest(new { message = result.ErrorMessage });
            }

            return Results.Ok(result.Data);
        });

        expenseGroup.MapDelete("/{id}", async (ClaimsPrincipal user, int id, IExpenseService expenseService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var isDeleted = await expenseService.DeleteExpenseAsync(userId, id);

            if (!isDeleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });
    }
}