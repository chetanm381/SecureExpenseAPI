using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;
using SecureExpenseAPI.DTOs.Categories;
using SecureExpenseAPI.Entities;
using SecureExpenseAPI.Utils;

namespace SecureExpenseAPI.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _dbContext;

    public CategoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(int userId)
    {
        return await _dbContext.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<(CategoryResponse? Data, string? ErrorMessage, bool IsConflict)> CreateCategoryAsync(int userId, CreateCategoryRequest request)
    {
        var validationResult = await CategoryValidationUtils.ValidateCategoryAsync(request.Name, userId, _dbContext);
        if (!validationResult.IsValid)
        {
            bool isConflict = validationResult.ErrorMessage?.Contains("already exists") == true;
            return (null, validationResult.ErrorMessage, isConflict);
        }

        var category = new Category
        {
            Name = request.Name,
            UserId = userId
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return (new CategoryResponse { Id = category.Id, Name = category.Name }, null, false);
    }

    public async Task<(CategoryResponse? Data, string? ErrorMessage, bool IsNotFound, bool IsConflict)> UpdateCategoryAsync(int userId, int id, UpdateCategoryRequest request)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
        {
            return (null, "Category not found", true, false);
        }

        var validationResult = await CategoryValidationUtils.ValidateCategoryAsync(request.Name, userId, _dbContext, id);
        if (!validationResult.IsValid)
        {
            bool isConflict = validationResult.ErrorMessage?.Contains("already exists") == true;
            return (null, validationResult.ErrorMessage, false, isConflict);
        }

        category.Name = request.Name;
        await _dbContext.SaveChangesAsync();

        return (new CategoryResponse { Id = category.Id, Name = category.Name }, null, false, false);
    }

    public async Task<bool> DeleteCategoryAsync(int userId, int id)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
        {
            return false;
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}