
namespace SecureExpenseAPI.DTOs.Expenses;
public class CreateExpenseRequest
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}