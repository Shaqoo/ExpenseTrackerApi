using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.KPI;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IKPIRecordService
    {
        Task<ApiResponse<KPIRecordDto>> CreateKPIRecordAsync(CreateKPIRecordRequest request);
        Task<ApiResponse<IEnumerable<KPIRecordDto>>> GetKPIRecordsForUserAsync();
    }
}