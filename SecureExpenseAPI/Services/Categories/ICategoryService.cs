using SecureExpenseAPI.DTOs.Categories;

namespace SecureExpenseAPI.Services.Categories;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(int userId);
    Task<(CategoryResponse? Data, string? ErrorMessage, bool IsConflict)> CreateCategoryAsync(int userId, CreateCategoryRequest request);
    Task<(CategoryResponse? Data, string? ErrorMessage, bool IsNotFound, bool IsConflict)> UpdateCategoryAsync(int userId, int id, UpdateCategoryRequest request);
    Task<bool> DeleteCategoryAsync(int userId, int id);
}