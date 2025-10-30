using ExpenseTracker.DTOS.Common;
using ExpenseTracker.DTOS.KPI;
using ExpenseTracker.Entities;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Helpers;
using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.UnitOfWork;

namespace ExpenseTracker.Services.Implementations
{
    public class KPIRecordService : IKPIRecordService
    {
        private readonly IKPIRecordRepository _kpiRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ILogger<KPIRecordService> _logger;

        public KPIRecordService(IKPIRecordRepository kpiRepository, IUnitOfWork unitOfWork, IAuthService authService, ILogger<KPIRecordService> logger)
        {
            _kpiRepository = kpiRepository;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _logger = logger;
        }

        public async Task<ApiResponse<KPIRecordDto>> CreateKPIRecordAsync(CreateKPIRecordRequest request)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                return ApiResponse<KPIRecordDto>.FailureResponse("User not authenticated.");
            }

            try
            {
                await _unitOfWork.BeginAsync();

                var kpiRecord = new KPIRecord
                {
                    UserId = userId.Value,
                    MetricName = request.MetricName,
                    Value = request.Value,
                    Currency = (request.Currency ?? "USD").ToUpper(),
                    Period = DateTime.UtcNow
                };

                await _kpiRepository.AddAsync(kpiRecord);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Created KPI Record {MetricName} for user {UserId}", request.MetricName, userId);
                return ApiResponse<KPIRecordDto>.SuccessResponse(MapToDto(kpiRecord));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating KPI record for user {UserId}", userId);
                return ApiResponse<KPIRecordDto>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        public async Task<ApiResponse<IEnumerable<KPIRecordDto>>> GetKPIRecordsForUserAsync()
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                return ApiResponse<IEnumerable<KPIRecordDto>>.FailureResponse("User not authenticated.");
            }

            try
            {
                var records = await _kpiRepository.GetByUserIdAsync(userId.Value);
                var dtos = records.Select(MapToDto);
                return ApiResponse<IEnumerable<KPIRecordDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting KPI records for user {UserId}", userId);
                return ApiResponse<IEnumerable<KPIRecordDto>>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        private static KPIRecordDto MapToDto(KPIRecord record) => new()
        {
            Id = record.Id,
            MetricName = record.MetricName,
            Value = record.Value,
            Currency = record.Currency,
            Period = record.Period
        };
    }
}