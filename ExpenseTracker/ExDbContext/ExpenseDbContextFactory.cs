namespace ExpenseTracker.ExDbContext
{
    using Humanizer.Configuration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    namespace ExpenseTracker.ExDbContext
    {
        public class ExpenseDbContextFactory : IDesignTimeDbContextFactory<ExpenseDbContext>
        {
            public ExpenseDbContext CreateDbContext(string[] args)
            {
                // Find the directory containing appsettings.json (usually the startup project)
                var basePath = Directory.GetCurrentDirectory();

                // Build configuration
                var config = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables()
                    .Build();


                var dbSettings = config.GetSection("DatabaseSettings");
                var connectionString = $"Host={dbSettings["DB_HOST"]};" +
                                       $"Port={dbSettings["DB_PORT"]};" +
                                       $"Username={dbSettings["DB_USERNAME"]};" +
                                       $"Password={dbSettings["DB_PASSWORD"]};" +
                                       $"Database={dbSettings["DB_NAME"]};";

               // config["ConnectionStrings:DefaultConnection"] = connectionString;

                var optionsBuilder = new DbContextOptionsBuilder<ExpenseDbContext>();
                optionsBuilder.UseNpgsql(connectionString);

                return new ExpenseDbContext(optionsBuilder.Options);
            }
        }
    }

}
