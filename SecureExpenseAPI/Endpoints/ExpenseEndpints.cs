
using SecureExpenseAPI.Data;
using SecureExpenseAPI.Utils;
using SecureExpenseAPI.DTOs.Expense;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SecureExpenseAPI.Entities;


namespace SecureExpenseAPI.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this WebApplication app)
    {
        var expenseGroup = app.MapGroup("/expenses").RequireAuthorization();

       expenseGroup.MapGet("/", async (ClaimsPrincipal user, AppDbContext dbContext) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var expenses = await dbContext.Expenses
                .Where(e => e.UserId == userId)
                .Select(e => new ExpenseResponse
                {
                    Id = e.Id,
                    Title = e.Title,
                    Amount = e.Amount,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();
            return Results.Ok(expenses);
        });

        expenseGroup.MapPost("/", async (ClaimsPrincipal user, CreateExpenseRequest request, AppDbContext dbContext) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var expense = new Expense
            {
                Title = request.Title,
                Amount = request.Amount,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Expenses.Add(expense);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/expenses/{expense.Id}", new ExpenseResponse
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                CreatedAt = expense.CreatedAt
            });
        });


        expenseGroup.MapDelete("/{id}", async (ClaimsPrincipal user, int id, AppDbContext dbContext) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            var expense = await dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (expense == null)
            {
                return Results.NotFound();
            }

            dbContext.Expenses.Remove(expense);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}