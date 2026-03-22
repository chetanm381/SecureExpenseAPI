namespace SecureExpenseAPI.DTOs.Summary;

public class SummaryResponse
{
    public decimal TotalAmount { get; set; }
    public int TotalCount { get; set; }
    public List<CategorySummary> Categories { get; set; } = new();
}

public class CategorySummary
{
    public string? CategoryName { get; set; }
    public decimal TotalAmount { get; set; }
}