namespace SecureExpenseAPI.DTOs.Expenses;

public class UpdateExpenseRequest
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? CategoryId { get; set; }
}