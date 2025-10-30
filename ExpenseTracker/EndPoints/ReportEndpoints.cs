using ExpenseTracker.DTOS.Report;
using ExpenseTracker.Enums;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Endpoints
{
   public static class ReportEndpoints
   {
       public static void MapReportEndpoints(this IEndpointRouteBuilder app)
       {
           var group = app.MapGroup("/api/reports").WithTags("Reports").RequireAuthorization();

           group.MapGet("/expenses", async ([AsParameters] ReportFilterRequest filters, [FromQuery] ReportFormat format, [FromServices] ICubeService cubeService) =>
           {
               var response = await cubeService.GenerateExpenseReportAsync(filters, format);
               return Results.Ok(response);
           })
           .WithSummary("Generates an expense report in the specified format (Json, Csv, Html).");
       }
   }
}