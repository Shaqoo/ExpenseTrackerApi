using ExpenseTracker.Entities;
using ExpenseTracker.Enums;
using ExpenseTracker.ExDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExpenseTracker.Configurations.DbInitializer
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
            var logger = scope.ServiceProvider
                              .GetRequiredService<ILoggerFactory>()
                              .CreateLogger("DatabaseInitializer");

            try
            {
                logger.LogInformation("Ensuring database exists...");
                EnsureDbCreation(context);
                logger.LogInformation("✅ Database ensured successfully.");

                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("✅ Database migrations applied successfully.");

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                logger.LogInformation("Seeding roles...");
                await SeedRolesAsync(roleManager, logger);
               
                logger.LogInformation("✅ Seeding completed successfully.");

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                
                if (!userManager.Users.Any())
                {
                    logger.LogInformation("Seeding initial data...");
                    await SeedAdminUserAsync(userManager, logger);
                    logger.LogInformation("✅ Seeding completed successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ An error occurred during database initialization.");
                throw;
            }
        }
        private static void EnsureDbCreation(ExpenseDbContext context)
        {
            if (context.Database.CanConnect())
                return;

            var databaseName = context.Database.GetDbConnection().Database;
            var connectionString = context.Database.GetConnectionString()!;

            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            builder.Database = "postgres";

            // var baseConnectionString = connectionString.Replace(
            //     $"Database={databaseName};", 
            //     string.Empty, 
            //     StringComparison.InvariantCultureIgnoreCase);

            using var connection = new NpgsqlConnection(builder.ConnectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
            var exists = cmd.ExecuteScalar();
            if (exists == null)
            {
                cmd.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
        private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager, ILogger logger)
        {
            string[] roles = { Role.Admin.ToString(), Role.User.ToString() };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    logger.LogInformation("Role '{Role}' created.", role);
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<User> userManager, ILogger logger)
        {
            var adminEmail = "admin@example.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    DefaultCurrency = "USD",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Role.Admin.ToString());
                    logger.LogInformation("Admin user created and assigned to Admin role.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError("Failed to create admin user: {Errors}", errors);
                }
            }
        }
    }
}
