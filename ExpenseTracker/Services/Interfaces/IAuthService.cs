using ExpenseTracker.DTOS.Auth;
using ExpenseTracker.DTOS.Common;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Guid? GetCurrentUserId();
    }
}