
using SecureExpenseAPI.Data;
using SecureExpenseAPI.Utils;
using SecureExpenseAPI.DTOs.Expenses;
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
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .Select(e => new ExpenseResponse
                {
                    Id = e.Id,
                    Title = e.Title,
                    Amount = e.Amount,
                    CreatedAt = e.CreatedAt,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category != null ? e.Category.Name : null
                })
                .ToListAsync();
            return Results.Ok(expenses);
        });

        expenseGroup.MapPost("/", async (ClaimsPrincipal user, CreateExpenseRequest request, AppDbContext dbContext) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId && c.UserId == userId);
                if (!categoryExists)
                {
                    return Results.BadRequest(new { message = "Invalid CategoryId or Category does not belong to user." });
                }
            }

            var expense = new Expense
            {
                Title = request.Title,
                Amount = request.Amount,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CategoryId = request.CategoryId
            };

            dbContext.Expenses.Add(expense);
            await dbContext.SaveChangesAsync();

            // Reload to get category info if needed, or just return from request
            string? categoryName = null;
            if (expense.CategoryId.HasValue)
            {
                var cat = await dbContext.Categories.FindAsync(expense.CategoryId);
                categoryName = cat?.Name;
            }

            return Results.Created($"/expenses/{expense.Id}", new ExpenseResponse
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                CreatedAt = expense.CreatedAt,
                CategoryId = expense.CategoryId,
                CategoryName = categoryName
            });
        });


        expenseGroup.MapGet("/{id}", async(ClaimsPrincipal user, int id, AppDbContext dbContext)=>{
            var userId = UserUtils.GetUserIdFromClaims(user);
            var expense = await dbContext.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e=>e.Id==id && e.UserId==userId);
                
            if(expense == null){
                return Results.NotFound();
            }
            return Results.Ok(new ExpenseResponse
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                CreatedAt = expense.CreatedAt,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category?.Name
            });
        });

        expenseGroup.MapPut("/{id}", async(ClaimsPrincipal user ,int id ,UpdateExpenseRequest request, AppDbContext dbContext)=>{
            var userId = UserUtils.GetUserIdFromClaims(user);
            
            var expense = await dbContext.Expenses.FirstOrDefaultAsync(e=>e.Id==id && e.UserId==userId);
            if(expense == null){
                return Results.NotFound();
            }

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId && c.UserId == userId);
                if (!categoryExists)
                {
                    return Results.BadRequest(new { message = "Invalid CategoryId or Category does not belong to user." });
                }
            }

            expense.Title = request.Title;
            expense.Amount = request.Amount;
            expense.CategoryId = request.CategoryId;

            await dbContext.SaveChangesAsync();

            // Load category name for response
            string? categoryName = null;
            if (expense.CategoryId.HasValue)
            {
                var cat = await dbContext.Categories.FindAsync(expense.CategoryId);
                categoryName = cat?.Name;
            }

            return Results.Ok(new ExpenseResponse
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                CreatedAt = expense.CreatedAt,
                CategoryId = expense.CategoryId,
                CategoryName = categoryName
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