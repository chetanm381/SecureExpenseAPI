using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;
using SecureExpenseAPI.DTOs.Expenses;
using SecureExpenseAPI.Entities;

namespace SecureExpenseAPI.Services.Expenses;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _dbContext;

    public ExpenseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ExpenseResponse>> GetExpensesAsync(int userId)
    {
        return await _dbContext.Expenses
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
    }

    public async Task<ExpenseResponse?> GetExpenseAsync(int userId, int id)
    {
        var expense = await _dbContext.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense == null) return null;

        return new ExpenseResponse
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            CreatedAt = expense.CreatedAt,
            CategoryId = expense.CategoryId,
            CategoryName = expense.Category?.Name
        };
    }

    public async Task<(ExpenseResponse? Data, string? ErrorMessage)> CreateExpenseAsync(int userId, CreateExpenseRequest request)
    {
        string? categoryName = null;

        if (request.CategoryId.HasValue)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);

            if (category == null)
            {
                return (null, "Invalid CategoryId or Category does not belong to user.");
            }
            categoryName = category.Name;
        }

        var expense = new Expense
        {
            Title = request.Title,
            Amount = request.Amount,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            CategoryId = request.CategoryId
        };

        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();

        return (new ExpenseResponse
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            CreatedAt = expense.CreatedAt,
            CategoryId = expense.CategoryId,
            CategoryName = categoryName
        }, null);
    }

    public async Task<(ExpenseResponse? Data, string? ErrorMessage)> UpdateExpenseAsync(int userId, int id, UpdateExpenseRequest request)
    {
        var expense = await _dbContext.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense == null)
        {
            return (null, "Expense not found.");
        }

        string? categoryName = expense.Category?.Name;

        // Optimized: only query Category if it's changing
        if (request.CategoryId != expense.CategoryId)
        {
            if (request.CategoryId.HasValue)
            {
                var category = await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);

                if (category == null)
                {
                    return (null, "Invalid CategoryId or Category does not belong to user.");
                }
                categoryName = category.Name;
            }
            else
            {
                categoryName = null;
            }
        }

        expense.Title = request.Title;
        expense.Amount = request.Amount;
        expense.CategoryId = request.CategoryId;

        await _dbContext.SaveChangesAsync();

        return (new ExpenseResponse
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            CreatedAt = expense.CreatedAt,
            CategoryId = expense.CategoryId,
            CategoryName = categoryName
        }, null);
    }

    public async Task<bool> DeleteExpenseAsync(int userId, int id)
    {
        var expense = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null) return false;

        _dbContext.Expenses.Remove(expense);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
