using ExpenseTracker.DTOS;
using ExpenseTracker.DTOS.Common;
using ExpenseTracker.Entities;
using ExpenseTracker.ExDbContext;
using ExpenseTracker.Services.Interfaces;
using MongoDB.Driver;

namespace ExpenseTracker.Services.Implementations
{
    public class MongoActivityLogService : IActivityLogService
    {
        //private readonly MongoDbContext _mongoDbContext;
        private static List<ActivityLog> ActivityLogs = [];
        private readonly ILogger<MongoActivityLogService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MongoActivityLogService(ILogger<MongoActivityLogService> logger, IHttpContextAccessor httpContextAccessor)
        {
           // _mongoDbContext = mongoDbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActivityAsync(Guid? userId, string action)
        {
            var activityLog = new ActivityLog
            {
                UserId = userId,
                Action = action
            };
            //await _mongoDbContext.ActivityLogs.InsertOneAsync(activityLog);
            ActivityLogs.Add(activityLog);
            await Task.CompletedTask;
            _logger.LogInformation("Logged activity for user {UserId}: {Action}", userId ?? Guid.Empty, action);
        }

        public async Task<ApiResponse<PagedResponse<ActivityLogDto>>> GetLogsAsync(PageRequest pageRequest, Guid? userId = null)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                _logger.LogWarning("GetLogsAsync was called outside of an HTTP request context.");
                return ApiResponse<PagedResponse<ActivityLogDto>>.FailureResponse("Could not determine user authentication status.");
            }
            var authService = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IAuthService>();
            var currentUserId = authService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return ApiResponse<PagedResponse<ActivityLogDto>>.FailureResponse("User not authenticated.");
            }
 
            try
            {
                // var filter = userId.HasValue
                //     ? Builders<ActivityLog>.Filter.Eq(log => log.UserId, userId.Value)
                //     : Builders<ActivityLog>.Filter.Empty;

                var filter = userId.HasValue
                ? ActivityLogs.Where(a => a.UserId == userId.Value)
                : ActivityLogs;

                //  var totalRecords = await _mongoDbContext.ActivityLogs.CountDocumentsAsync(filter);

                var totalRecords = ActivityLogs.Count();
                // var logs = await _mongoDbContext.ActivityLogs
                //     .Find(filter)
                //     .SortByDescending(log => log.Timestamp)
                //     .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                //     .Limit(pageRequest.PageSize)
                //     .ToListAsync();

                var logs = filter.OrderByDescending(log => log.Timestamp)
                    .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize);

                var logDtos = logs.Select(MapToDto);

                var pagedResponse = new PagedResponse<ActivityLogDto>(logDtos, pageRequest.PageNumber, pageRequest.PageSize, (int)totalRecords);
                

                return ApiResponse<PagedResponse<ActivityLogDto>>.SuccessResponse(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching activity logs for user {UserId}.", currentUserId);
                return ApiResponse<PagedResponse<ActivityLogDto>>.FailureResponse("An error occurred while fetching logs.");
            }
        }

        private static ActivityLogDto MapToDto(ActivityLog log)
        {
            return new ActivityLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                Timestamp = log.Timestamp
            };
        }
    }
}