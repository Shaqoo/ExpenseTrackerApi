using ExpenseTracker.BackgroundServices;
using ExpenseTracker.ExternalServices;
using ExpenseTracker.Services;
using ExpenseTracker.Services.Implementations;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceRegistration(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IActivityLogService, MongoActivityLogService>();
        services.AddScoped<ICubeService, CubeService>();
        services.AddScoped<IExchangeRateUpdaterService, ExchangeRateUpdaterService>();
            // Add a singleton to signal when the app is ready
        services.AddSingleton<ApplicationStartupState>();

        // Register the background service
        services.AddHostedService<ExchangeRateUpdaterJob>();
        
    

        return services;
    }
}