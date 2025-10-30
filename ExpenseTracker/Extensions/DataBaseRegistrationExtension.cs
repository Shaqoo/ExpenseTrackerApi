using ExpenseTracker.Entities;
using ExpenseTracker.ExDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Extensions
{
    public static class DataBaseRegistrationExtension
    {
        public static IServiceCollection AddDatabaseRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var dbSettings = configuration.GetSection("DatabaseSettings");
            var connectionString = $"Host={dbSettings["DB_HOST"]};" +
                                   $"Port={dbSettings["DB_PORT"]};" +
                                   $"Username={dbSettings["DB_USERNAME"]};" +
                                   $"Password={dbSettings["DB_PASSWORD"]};" +
                                   $"Database={dbSettings["DB_NAME"]};";

            configuration["ConnectionStrings:DefaultConnection"] = connectionString;

            services.AddDbContext<ExpenseDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ExpenseDbContext>().AddDefaultTokenProviders();

            return services;
        }
    }
}