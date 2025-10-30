using ExpenseTracker.DTOS.Auth;
using ExpenseTracker.DTOS.Common;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints
{
   public static class AuthEndpoints
   {
       public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
       {
           var group = app.MapGroup("/api/auth").WithTags("Authentication");

           group.MapPost("/login", async ([FromBody] LoginRequest request, [FromServices] IAuthService authService) =>
           {
               var response = await authService.LoginAsync(request);
               return response.Success ? Results.Ok(response) : Results.BadRequest(response);
           })
           .WithSummary("Logs in a user and returns a JWT token.");
       }
   }
}