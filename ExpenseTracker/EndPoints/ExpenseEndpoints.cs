using ExpenseTracker.DTOS.Expense;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints
{
   public static class ExpenseEndpoints
   {
       public static void MapExpenseEndpoints(this IEndpointRouteBuilder app)
       {
           var group = app.MapGroup("/api/expenses").WithTags("Expenses").RequireAuthorization();

           group.MapPost("/", async ([FromBody] CreateExpenseRequest request, [FromServices] IExpenseService expenseService) =>
           {
               var response = await expenseService.CreateExpenseAsync(request);
               return response.Success ? Results.Created($"/api/expenses/{response.Data?.Id}", response) : Results.BadRequest(response);
           })
           .WithSummary("Creates a new expense for the authenticated user.");

           group.MapGet("/", async ([FromServices] IExpenseService expenseService) =>
           {
               var response = await expenseService.GetExpensesForUserAsync();
               return Results.Ok(response);
           })
           .WithSummary("Gets all expenses for the authenticated user.");

           group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateExpenseRequest request, [FromServices] IExpenseService expenseService) =>
           {
               var response = await expenseService.UpdateExpenseAsync(id, request);
               return response.Success ? Results.Ok(response) : Results.BadRequest(response);
           })
           .WithSummary("Updates an existing expense.");

           group.MapDelete("/{id:guid}", async (Guid id, [FromServices] IExpenseService expenseService) =>
           {
               var response = await expenseService.DeleteExpenseAsync(id);
               return response.Success ? Results.Ok(response) : Results.NotFound(response);
           })
           .WithSummary("Deletes an expense.");
       }
   }
}