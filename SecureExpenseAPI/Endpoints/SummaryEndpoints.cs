using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;
using SecureExpenseAPI.DTOs.Summary;
using SecureExpenseAPI.Entities;
using SecureExpenseAPI.Utils;

namespace SecureExpenseAPI.Endpoints;
public static class SummaryEndpoints
{
    public static void MapSummaryEndpoints(this WebApplication app)
    {
        app.MapGet("/summary", async (AppDbContext dbContext,  ClaimsPrincipal user) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);

            var expenses = await dbContext.Expenses
                .Where(e => e.UserId == userId)
                .Include(e => e.Category)
                .ToListAsync();

            var summary = new SummaryResponse
            {
                TotalAmount = expenses.Sum(e => e.Amount),
                TotalCount = expenses.Count,
                Categories = expenses
                    .Where(e => e.Category != null && !string.IsNullOrEmpty(e.Category?.Name))
                    .GroupBy(e => e.Category?.Name ?? "Uncategorized")
                    .OrderByDescending(g => g.Sum(e => e.Amount))
                    .Select(g => new CategorySummary
                    
                    {
                        CategoryName = g.Key,
                        TotalAmount = g.Sum(e => e.Amount)
                    })
                    .ToList()
            };

            return Results.Ok(summary);
        })
        .RequireAuthorization();
    }
}