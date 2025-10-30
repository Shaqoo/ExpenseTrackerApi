using System.Text;
using System.Text.Json;
using ExpenseTracker.DTOS.Report;
using ExpenseTracker.Enums;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services.Implementations
{
    public class CubeService : ICubeService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IAuthService _authService;
        private readonly ILogger<CubeService> _logger;

        public CubeService(IExpenseRepository expenseRepository, IAuthService authService, ILogger<CubeService> logger)
        {
            _expenseRepository = expenseRepository;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> GenerateExpenseReportAsync(ReportFilterRequest filters, ReportFormat format)
        {
            var userId = _authService.GetCurrentUserId();
            if (userId == null)
            {
                return new UnauthorizedObjectResult("User not authenticated.");
            }

            try
            {
                var expenses = await _expenseRepository.GetByUserIdAsync(userId.Value);

                var filteredExpenses = expenses
                    .Where(e => !filters.StartDate.HasValue || e.Date >= filters.StartDate.Value)
                    .Where(e => !filters.EndDate.HasValue || e.Date <= filters.EndDate.Value)
                    .ToList();

                _logger.LogInformation("Generating {Format} report for user {UserId} with {Count} records.", format, userId, filteredExpenses.Count);

                var reportData = filteredExpenses
                    .GroupBy(e => e.CategoryId)
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        TotalAmountUSD = g.Sum(e => e.ConvertedAmountUSD),
                        TransactionCount = g.Count()
                    })
                    .ToList();

                return format switch
                {
                    ReportFormat.Json => new OkObjectResult(reportData),
                    ReportFormat.Csv => GenerateCsvResult(reportData),
                    ReportFormat.Html => GenerateHtmlResult(reportData),
                    _ => new BadRequestObjectResult("Unsupported report format.")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report for user {UserId}", userId);
                return new ObjectResult("An internal error occurred while generating the report.") { StatusCode = 500 };
            }
        }

        private static IActionResult GenerateCsvResult(IEnumerable<object> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("CategoryId,TotalAmountUSD,TransactionCount");

            foreach (var item in data)
            {
                var categoryId = item.GetType().GetProperty("CategoryId")?.GetValue(item, null);
                var totalAmount = item.GetType().GetProperty("TotalAmountUSD")?.GetValue(item, null);
                var count = item.GetType().GetProperty("TransactionCount")?.GetValue(item, null);
                sb.AppendLine($"{categoryId},{totalAmount},{count}");
            }

            return new FileContentResult(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv")
            {
                FileDownloadName = $"report_{DateTime.UtcNow:yyyyMMddHHmmss}.csv"
            };
        }

        private static IActionResult GenerateHtmlResult(IEnumerable<object> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><title>Expense Report</title><style>table, th, td { border: 1px solid black; border-collapse: collapse; padding: 5px; } th { background-color: #f2f2f2; }</style></head><body>");
            sb.AppendLine($"<h1>Expense Report - {DateTime.UtcNow:yyyy-MM-dd}</h1>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>CategoryId</th><th>Total Amount (USD)</th><th>Transaction Count</th></tr>");

            foreach (var item in data)
            {
                var categoryId = item.GetType().GetProperty("CategoryId")?.GetValue(item, null);
                var totalAmount = item.GetType().GetProperty("TotalAmountUSD")?.GetValue(item, null);
                var count = item.GetType().GetProperty("TransactionCount")?.GetValue(item, null);
                sb.AppendLine($"<tr><td>{categoryId}</td><td>{totalAmount:C}</td><td>{count}</td></tr>");
            }

            sb.AppendLine("</table></body></html>");

            return new ContentResult { Content = sb.ToString(), ContentType = "text/html" };
        }
    }
}