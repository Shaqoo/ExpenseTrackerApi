using ExpenseTracker.DTOS.Category;
using ExpenseTracker.DTOS.Common;
using ExpenseTracker.Entities;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.Services.Helpers;
using ExpenseTracker.UnitOfWork;

namespace ExpenseTracker.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ILogger<CategoryService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IActivityLogService _activityLogService;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ILogger<CategoryService> logger, IAuthService authService, ICacheService cacheService, IActivityLogService activityLogService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authService = authService;
            _cacheService = cacheService;
            _activityLogService = activityLogService;
        }

        public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                return ApiResponse<CategoryDto>.FailureResponse("User not authenticated.");
            }

            try
            {
                var existingCategory = await _categoryRepository.GetByNameAsync(request.Name, userId.Value);
                if (existingCategory != null)
                {
                    return ApiResponse<CategoryDto>.FailureResponse($"A category with the name '{request.Name}' already exists.");
                }

                await _unitOfWork.BeginAsync();

                var category = new Category
                {
                    Name = request.Name,
                    Description = request.Description ?? "",
                    UserId = userId.Value
                };

                await _categoryRepository.AddAsync(category);
                await _unitOfWork.CommitAsync();

                await _activityLogService.LogActivityAsync(userId, $"Created category '{category.Name}'.");
                await _cacheService.RemoveAsync(CacheKeys.Categories(userId.Value));

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "An error occurred while creating a category for user {UserId}.", userId);
                return ApiResponse<CategoryDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid categoryId)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                return ApiResponse<bool>.FailureResponse("User not authenticated.");
            }

            try
            {
                var category = await _categoryRepository.GetByIdAndUserIdAsync(categoryId, userId.Value);
                if (category == null)
                {
                    return ApiResponse<bool>.FailureResponse("Category not found or you do not have permission to delete it.");
                }

                await _unitOfWork.BeginAsync();
                await _categoryRepository.DeleteAsync(category);
                await _unitOfWork.CommitAsync();

                await _activityLogService.LogActivityAsync(userId, $"Deleted category '{category.Name}'.");
                await _cacheService.RemoveAsync(CacheKeys.Categories(userId.Value));

                return ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "An error occurred while deleting category {CategoryId} for user {UserId}.", categoryId, userId);
                return ApiResponse<bool>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesForUserAsync()
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                return ApiResponse<IEnumerable<CategoryDto>>.FailureResponse("User not authenticated.");
            }

            try
            {
                string cacheKey = CacheKeys.Categories(userId.Value);
                var cachedCategories = await _cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
                if (cachedCategories != null)
                {
                    return ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(cachedCategories);
                }

                var categories = await _categoryRepository.GetByUserIdAsync(userId.Value);
                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                });

                await _cacheService.SetAsync(cacheKey, categoryDtos, TimeSpan.FromHours(1));

                return ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching categories for user {UserId}.", userId);
                return ApiResponse<IEnumerable<CategoryDto>>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                return ApiResponse<CategoryDto>.FailureResponse("User not authenticated.");
            }

            try
            {
                var category = await _categoryRepository.GetByIdAndUserIdAsync(categoryId, userId.Value);
                if (category == null)
                {
                    return ApiResponse<CategoryDto>.FailureResponse("Category not found or you do not have permission to update it.");
                }

                await _unitOfWork.BeginAsync();

                category.Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : category.Name;
                category.Description = request.Description ?? category.Description;

                await _categoryRepository.UpdateAsync(category);
                await _unitOfWork.CommitAsync();

                await _activityLogService.LogActivityAsync(userId, $"Updated category '{category.Name}'.");
                await _cacheService.RemoveAsync(CacheKeys.Categories(userId.Value));

                var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name, Description = category.Description };
                return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "An error occurred while updating category {CategoryId} for user {UserId}.", categoryId, userId);
                return ApiResponse<CategoryDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }
    }
}