using ExpenseTracker.DTOS.Category;
using ExpenseTracker.DTOS.Common;

namespace ExpenseTracker.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesForUserAsync();
        Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request);
        Task<ApiResponse<bool>> DeleteCategoryAsync(Guid categoryId);
    }
}