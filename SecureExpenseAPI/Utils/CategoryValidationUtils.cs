using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;

namespace SecureExpenseAPI.Utils;

public static class CategoryValidationUtils
{
    private const int MaxCategoryNameLength = 100;

    /// <summary>
    /// Validates category name length and presence
    /// </summary>
    public static ValidationResult ValidateNameFormat(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationResult.Failure("Category name is required");
        }

        if (name.Length > MaxCategoryNameLength)
        {
            return ValidationResult.Failure($"Category name cannot exceed {MaxCategoryNameLength} characters");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates if category name is unique for a specific user
    /// </summary>
    public static async Task<ValidationResult> ValidateNameUniquenessAsync(string name, int userId, AppDbContext dbContext, int? currentCategoryId = null)
    {
        var query = dbContext.Categories
            .Where(c => c.UserId == userId && c.Name.ToLower() == name.ToLower());

        // If updating, exclude the current category from the uniqueness check
        if (currentCategoryId.HasValue)
        {
            query = query.Where(c => c.Id != currentCategoryId.Value);
        }

        if (await query.AnyAsync())
        {
            return ValidationResult.Failure($"Category with name '{name}' already exists");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Performs full validation for a category (Format + Uniqueness)
    /// </summary>
    public static async Task<ValidationResult> ValidateCategoryAsync(string name, int userId, AppDbContext dbContext, int? currentCategoryId = null)
    {
        var formatResult = ValidateNameFormat(name);
        if (!formatResult.IsValid)
        {
            return formatResult;
        }

        return await ValidateNameUniquenessAsync(name, userId, dbContext, currentCategoryId);
    }
}
