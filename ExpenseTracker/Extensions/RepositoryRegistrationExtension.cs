using ExpenseTracker.Entities;
using ExpenseTracker.Repositories.Implementations;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Extensions
{
    public static class RepositoryRegistrationExtension
    {
        public static IServiceCollection AddRepoRegistration(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
            services.AddScoped<IKPIRecordRepository, KPIRecordRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            return services;
        }
    }
}
