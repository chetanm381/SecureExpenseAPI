
namespace SecureExpenseAPI.DTOs.Expense;
public class CreateExpenseRequest
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}