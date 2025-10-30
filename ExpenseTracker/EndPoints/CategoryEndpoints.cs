using ExpenseTracker.DTOS.Category;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints
{
   public static class CategoryEndpoints
   {
       public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
       {
           var group = app.MapGroup("/api/categories").WithTags("Categories").RequireAuthorization();

           group.MapPost("/", async ([FromBody] CreateCategoryRequest request, [FromServices] ICategoryService categoryService) =>
           {
               var response = await categoryService.CreateCategoryAsync(request);
               return response.Success ? Results.Created($"/api/categories/{response.Data?.Id}", response) : Results.BadRequest(response);
           })
           .WithSummary("Creates a new category for the authenticated user.");

           group.MapGet("/", async ([FromServices] ICategoryService categoryService) =>
           {
               var response = await categoryService.GetCategoriesForUserAsync();
               return Results.Ok(response);
           })
           .WithSummary("Gets all categories for the authenticated user.");

           group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateCategoryRequest request, [FromServices] ICategoryService categoryService) =>
           {
               var response = await categoryService.UpdateCategoryAsync(id, request);
               return response.Success ? Results.Ok(response) : Results.BadRequest(response);
           })
           .WithSummary("Updates an existing category.");

           group.MapDelete("/{id:guid}", async (Guid id, [FromServices] ICategoryService categoryService) =>
           {
               var response = await categoryService.DeleteCategoryAsync(id);
               return response.Success ? Results.Ok(response) : Results.NotFound(response);
           })
           .WithSummary("Deletes a category.");
       }
   }
}