using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.Expense;
using ExpenseTracker.Entities;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Helpers;
using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.UnitOfWork;

namespace ExpenseTracker.Services.Implementations
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ILogger<ExpenseService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IActivityLogService _activityLogService;

        public ExpenseService(
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository,
            IExchangeRateService exchangeRateService,
            IUnitOfWork unitOfWork,
            ILogger<ExpenseService> logger,
            IAuthService authService, ICacheService cacheService, IActivityLogService activityLogService)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _exchangeRateService = exchangeRateService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authService = authService;
            _cacheService = cacheService;
            _activityLogService = activityLogService;
        }

        public async Task<ApiResponse<ExpenseDto>> CreateExpenseAsync(CreateExpenseRequest request)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("CreateExpenseAsync call failed: User not authenticated.");
                return ApiResponse<ExpenseDto>.FailureResponse("User not authenticated.");
            }

            try
            {
                _logger.LogInformation("Attempting to create expense for user {UserId}", userId);

                var category = await _categoryRepository.GetByIdAndUserIdAsync(request.CategoryId, userId.Value);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found for user {UserId}", request.CategoryId, userId);
                    return ApiResponse<ExpenseDto>.FailureResponse("Category not found.");
                }

                var conversionResult = await _exchangeRateService.ConvertAmountAsync(request.Amount, request.Currency, "USD");
                if (!conversionResult.Success)
                {
                    _logger.LogError("Currency conversion failed for user {UserId} from {FromCurrency} to USD. Reason: {Reason}", userId, request.Currency, conversionResult.Message);
                    return ApiResponse<ExpenseDto>.FailureResponse($"Currency conversion failed: {conversionResult.Message}");
                }

                var expense = new Expense
                {
                    UserId = userId.Value,
                    CategoryId = request.CategoryId,
                    Title = request.Title,
                    Amount = request.Amount,
                    Currency = request.Currency.ToUpper(),
                    ConvertedAmountUSD = conversionResult.Data,
                    Type = request.Type,
                    Date = request.Date,
                    Notes = request.Notes ?? string.Empty
                };

                await _unitOfWork.BeginAsync();
                await _expenseRepository.AddAsync(expense);
                await _unitOfWork.CommitAsync();

                await _activityLogService.LogActivityAsync(userId, $"Created expense '{expense.Title}'.");
                await _cacheService.RemoveAsync(CacheKeys.Expenses(userId.Value));

                _logger.LogInformation("Successfully created expense with ID {ExpenseId} for user {UserId}", expense.Id, userId);

                var expenseDto = MapToDto(expense);
                return ApiResponse<ExpenseDto>.SuccessResponse(expenseDto, "Expense created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "An unexpected error occurred while creating an expense for user {UserId}", userId);
                return ApiResponse<ExpenseDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<bool>> DeleteExpenseAsync(Guid expenseId)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("DeleteExpenseAsync call failed: User not authenticated.");
                return ApiResponse<bool>.FailureResponse("User not authenticated.");
            }

            try
            {
                _logger.LogInformation("Attempting to delete expense {ExpenseId} for user {UserId}", expenseId, userId);

                var expense = await _expenseRepository.GetByIdAsync(expenseId);
                if (expense == null || expense.UserId != userId.Value)
                {
                    _logger.LogWarning("Expense {ExpenseId} not found or user {UserId} does not have permission to delete.", expenseId, userId);
                    return ApiResponse<bool>.FailureResponse("Expense not found or you do not have permission to delete it.");
                }

                await _unitOfWork.BeginAsync();
                _expenseRepository.Remove(expense);
                await _unitOfWork.CommitAsync();

                await _activityLogService.LogActivityAsync(userId, $"Deleted expense '{expense.Title}'.");
                await _cacheService.RemoveAsync(CacheKeys.Expenses(userId.Value));

                _logger.LogInformation("Successfully deleted expense {ExpenseId} for user {UserId}", expenseId, userId);
                return ApiResponse<bool>.SuccessResponse(true, "Expense deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "An error occurred while deleting expense {ExpenseId} for user {UserId}", expenseId, userId);
                return ApiResponse<bool>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<IEnumerable<ExpenseDto>>> GetExpensesForUserAsync()
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("GetExpensesForUserAsync call failed: User not authenticated.");
                return ApiResponse<IEnumerable<ExpenseDto>>.FailureResponse("User not authenticated.");
            }

            try
            {
                string cacheKey = CacheKeys.Expenses(userId.Value);
                var cachedExpenses = await _cacheService.GetAsync<IEnumerable<ExpenseDto>>(cacheKey);
                if (cachedExpenses != null)
                {
                    _logger.LogInformation("Found {Count} expenses in cache for user {UserId}", cachedExpenses.Count(), userId);
                    return ApiResponse<IEnumerable<ExpenseDto>>.SuccessResponse(cachedExpenses);
                }

                _logger.LogInformation("Fetching all expenses for user {UserId}", userId);
                var expenses = await _expenseRepository.GetByUserIdAsync(userId.Value);
                var expenseDtos = expenses.Select(MapToDto);

                await _cacheService.SetAsync(cacheKey, expenseDtos, TimeSpan.FromMinutes(30));

                _logger.LogInformation("Found {Count} expenses for user {UserId}", expenseDtos.Count(), userId);
                return ApiResponse<IEnumerable<ExpenseDto>>.SuccessResponse(expenseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching expenses for user {UserId}", userId);
                return ApiResponse<IEnumerable<ExpenseDto>>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<ExpenseDto>> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("UpdateExpenseAsync call failed: User not authenticated.");
                return ApiResponse<ExpenseDto>.FailureResponse("User not authenticated.");
            }

            try
            {
                _logger.LogInformation("Attempting to update expense {ExpenseId} for user {UserId}", expenseId, userId);

                var expense = await _expenseRepository.GetByIdAsync(expenseId);
                if (expense == null || expense.UserId != userId.Value)
                {
                    _logger.LogWarning("Expense {ExpenseId} not found or user {UserId} does not have permission to update.", expenseId, userId);
                    return ApiResponse<ExpenseDto>.FailureResponse("Expense not found or you do not have permission to update it.");
                }

                await _unitOfWork.BeginAsync();

                if (request.CategoryId.HasValue)
                {
                    var category = await _categoryRepository.GetByIdAndUserIdAsync(request.CategoryId.Value, userId.Value);
                    if (category == null)
                    {
                        _logger.LogWarning("Update failed: New category {CategoryId} not found for user {UserId}", request.CategoryId.Value, userId);
                        return ApiResponse<ExpenseDto>.FailureResponse("The specified category was not found.");
                    }
                    expense.CategoryId = request.CategoryId.Value;
                }

                expense.Title = !string.IsNullOrWhiteSpace(request.Title) ? request.Title : expense.Title;
                expense.Date = request.Date ?? expense.Date;
                expense.Notes = request.Notes ?? expense.Notes;

                if (request.Amount.HasValue && request.Amount.Value != expense.Amount)
                {
                    var conversionResult = await _exchangeRateService.ConvertAmountAsync(request.Amount.Value, expense.Currency, "USD");
                    if (!conversionResult.Success)
                    {
                        _logger.LogError("Currency conversion failed during update for user {UserId}. Reason: {Reason}", userId, conversionResult.Message);
                        return ApiResponse<ExpenseDto>.FailureResponse($"Currency conversion failed: {conversionResult.Message}");
                    }
                    expense.Amount = request.Amount.Value;
                    expense.ConvertedAmountUSD = conversionResult.Data;
                }

                _expenseRepository.Update(expense);
                await _unitOfWork.CommitAsync();

                await _activityLogService.LogActivityAsync(userId, $"Updated expense '{expense.Title}'.");
                await _cacheService.RemoveAsync(CacheKeys.Expenses(userId.Value));

                _logger.LogInformation("Successfully updated expense {ExpenseId} for user {UserId}", expenseId, userId);
                var expenseDto = MapToDto(expense);
                return ApiResponse<ExpenseDto>.SuccessResponse(expenseDto, "Expense updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "An error occurred while updating expense {ExpenseId} for user {UserId}", expenseId, userId);
                return ApiResponse<ExpenseDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        private static ExpenseDto MapToDto(Expense expense)
        {
            return new ExpenseDto
            {
                Id = expense.Id,
                CategoryId = expense.CategoryId,
                Title = expense.Title,
                Amount = expense.Amount,
                Currency = expense.Currency,
                ConvertedAmountUSD = expense.ConvertedAmountUSD,
                Type = expense.Type,
                Date = expense.Date,
                Notes = expense.Notes
            };
        }
    }
}