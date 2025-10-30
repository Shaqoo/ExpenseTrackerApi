using ExpenseTracker.DTOS.Report;
using ExpenseTracker.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Services.Interfaces
{
    public interface ICubeService
    {
        Task<IActionResult> GenerateExpenseReportAsync(ReportFilterRequest filters, ReportFormat format);
    }
}