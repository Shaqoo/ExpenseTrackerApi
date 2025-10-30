using ExpenseTracker.DTOS.User;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/users").WithTags("Users");

            group.MapPost("/register", async ([FromBody] CreateUserRequest request, [FromServices] IUserService userService) =>
            {
                var response = await userService.RegisterUserAsync(request);
                return response.Success ? Results.Created($"/api/users/{response.Data?.Id}", response) : Results.BadRequest(response);
            })
            .WithSummary("Registers a new user.");

            group.MapGet("/{id:guid}", async (Guid id, [FromServices] IUserService userService) =>
            {
                var response = await userService.GetUserByIdAsync(id);
                return response.Success ? Results.Ok(response) : Results.NotFound(response);
            })
            .RequireAuthorization()
            .WithSummary("Gets a user's profile by their ID.");

            group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateUserRequest request, [FromServices] IUserService userService) =>
            {
                var response = await userService.UpdateUserAsync(id, request);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            })
            .RequireAuthorization()
            .WithSummary("Updates a user's profile.");
        }
    }
}