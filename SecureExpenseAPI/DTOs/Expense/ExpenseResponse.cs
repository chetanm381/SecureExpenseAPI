

namespace SecureExpenseAPI.DTOs.Expenses;

public class ExpenseResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
  