using System.Security.Claims;
using ExpenseTracker.DTOS;
using ExpenseTracker.Enums;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Endpoints
{
   public static class ActivityLogEndpoints
   {
       public static void MapActivityLogEndpoints(this IEndpointRouteBuilder app)
       {
           var group = app.MapGroup("/api/logs").WithTags("Activity Logs").RequireAuthorization();

           group.MapGet("/", async (
               [AsParameters] PageRequest pageRequest,
               IActivityLogService logService,
               IAuthService authService,
               ClaimsPrincipal user) =>
           {
               var targetUserId = user.IsInRole(Role.Admin.ToString()) ? (Guid?)null : authService.GetCurrentUserId();

               var response = await logService.GetLogsAsync(pageRequest, targetUserId);
               return Results.Ok(response);
           })
           .WithSummary("Gets activity logs. Admins see all logs; users see their own.");
       }
   }
}