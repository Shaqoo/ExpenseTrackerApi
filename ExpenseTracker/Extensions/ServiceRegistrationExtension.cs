using ExpenseTracker.BackgroundServices;
using ExpenseTracker.ExDbContext;
using ExpenseTracker.ExternalServices;
using ExpenseTracker.Services.Implementations;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Extensions
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddServicesRegistration(this IServiceCollection services)
        {
            services.AddHttpClient<IExchangeRateUpdaterService, ExchangeRateUpdaterService>();

            //services.AddSingleton<MongoDbContext>();
            services.AddScoped<IActivityLogService, MongoActivityLogService>();
            services.AddSingleton<ICacheService, RedisCacheService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IExchangeRateService, ExchangeRateService>();
            services.AddScoped<IKPIRecordService, KPIRecordService>();
            services.AddScoped<ICubeService, CubeService>();
            services.AddHostedService<ExchangeRateUpdaterJob>();


            return services;
        }
    }
}