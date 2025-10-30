using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.User;
using ExpenseTracker.Entities;
using ExpenseTracker.Enums;
using ExpenseTracker.Services.Helpers;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IAuthService _authService;

        public UserService(UserManager<User> userManager, ILogger<UserService> logger, IAuthService authService)
        {
            _userManager = userManager;
            _logger = logger;
            _authService = authService;
        }

        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId)
        {
            var currentUserId = _authService.GetCurrentUserId();
            if (currentUserId == null || currentUserId.Value != userId)
            {
                _logger.LogWarning("Unauthorized attempt to access user data for {TargetUserId} by user {CurrentUserId}", userId, currentUserId);
                return ApiResponse<UserDto>.FailureResponse("You are not authorized to view this user's information.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return ApiResponse<UserDto>.FailureResponse("User not found.");
                }

                var userDto = MapToDto(user);
                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user {UserId}", userId);
                return ApiResponse<UserDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<UserDto>> RegisterUserAsync(CreateUserRequest request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: Email {Email} is already in use.", request.Email);
                    return ApiResponse<UserDto>.FailureResponse("An account with this email address already exists.");
                }

                var user = new User
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    DefaultCurrency = (request.DefaultCurrency ?? "USD").ToUpper()
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("User registration failed for {Email}. Errors: {Errors}", request.Email, errors);
                    return ApiResponse<UserDto>.FailureResponse(errors);
                }

                await _userManager.AddToRoleAsync(user, Role.User.ToString());

                _logger.LogInformation("User {Email} registered successfully with ID {UserId}", user.Email, user.Id);
                var userDto = MapToDto(user);
                return ApiResponse<UserDto>.SuccessResponse(userDto, "User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during user registration for email {Email}", request.Email);
                return ApiResponse<UserDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request)
        {
            var currentUserId = _authService.GetCurrentUserId();
            if (currentUserId == null || currentUserId.Value != userId)
            {
                return ApiResponse<UserDto>.FailureResponse("You are not authorized to update this user's information.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ApiResponse<UserDto>.FailureResponse("User not found.");
            }

            user.FullName = !string.IsNullOrWhiteSpace(request.FullName) ? request.FullName : user.FullName;
            user.DefaultCurrency = !string.IsNullOrWhiteSpace(request.DefaultCurrency) ? request.DefaultCurrency.ToUpper() : user.DefaultCurrency;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User update failed for {UserId}. Errors: {Errors}", userId, errors);
                return ApiResponse<UserDto>.FailureResponse(errors);
            }

            _logger.LogInformation("User profile {UserId} updated successfully.", userId);
            return ApiResponse<UserDto>.SuccessResponse(MapToDto(user), "Profile updated successfully.");
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                DefaultCurrency = user.DefaultCurrency,
                CreatedAt = user.CreatedAt
            };
        }
    }
}