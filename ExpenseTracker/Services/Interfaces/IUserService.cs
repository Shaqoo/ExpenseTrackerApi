using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.User;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> RegisterUserAsync(CreateUserRequest request);
        Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId);
        Task<ApiResponse<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    }
}