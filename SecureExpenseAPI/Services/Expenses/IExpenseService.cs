using SecureExpenseAPI.DTOs.Expenses;

namespace SecureExpenseAPI.Services.Expenses;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseResponse>> GetExpensesAsync(int userId);
    Task<ExpenseResponse?> GetExpenseAsync(int userId, int id);
    Task<(ExpenseResponse? Data, string? ErrorMessage)> CreateExpenseAsync(int userId, CreateExpenseRequest request);
    Task<(ExpenseResponse? Data, string? ErrorMessage)> UpdateExpenseAsync(int userId, int id, UpdateExpenseRequest request);
    Task<bool> DeleteExpenseAsync(int userId, int id);
}
