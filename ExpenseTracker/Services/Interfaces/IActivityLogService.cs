using ExpenseTracker.DTOS;
using ExpenseTracker.DTOS.Common;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogActivityAsync(Guid? userId, string action);
        Task<ApiResponse<PagedResponse<ActivityLogDto>>> GetLogsAsync(PageRequest pageRequest, Guid? userId = null);
    }
}