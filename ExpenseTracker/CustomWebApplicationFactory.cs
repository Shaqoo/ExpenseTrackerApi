// using Microsoft.AspNetCore.Mvc.Testing;
// using Testcontainers.MongoDb;
// using Testcontainers.PostgreSql;
// using Testcontainers.Redis;
// using Xunit;

// namespace ExpenseTracker.Tests;

// public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
// {
//     private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
//         .WithImage("postgres:15")
//         .WithDatabase("test_db")
//         .WithUsername("test_user")
//         .WithPassword("test_password")
//         .Build();

//     private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
//         .WithImage("mongo:latest")
//         .Build();

//     private readonly RedisContainer _redisContainer = new RedisBuilder()
//         .WithImage("redis:latest")
//         .Build();

//     protected override void ConfigureWebHost(IWebHostBuilder builder)
//     {
//         builder.ConfigureAppConfiguration(config =>
//         {
//             config.AddInMemoryCollection(new Dictionary<string, string?>
//             {
//                 ["ConnectionStrings:DefaultConnection"] = _postgresContainer.GetConnectionString(),
//                 ["ConnectionStrings:MongoDb"] = _mongoContainer.GetConnectionString(), // No need to modify this, MongoDB driver handles connection retries.
//                 ["ConnectionStrings:Redis"] = $"{_redisContainer.GetConnectionString()},abortConnect=false",
//                 ["JwtSettings:Secret"] = "ThisIsAStrongAndLongSecretForIntegrationTesting!",
//                 ["DatabaseSettings:DB_HOST"] = null,
//                 ["DatabaseSettings:DB_PORT"] = null,
//                 ["DatabaseSettings:DB_USERNAME"] = null,
//                 ["DatabaseSettings:DB_PASSWORD"] = null,
//                 ["DatabaseSettings:DB_NAME"] = null,
//             });
//         });
//     }

//     public async Task InitializeAsync()
//     {
//         await _postgresContainer.StartAsync();
//         await _mongoContainer.StartAsync();
//         await _redisContainer.StartAsync();
//     }

//     public new async Task DisposeAsync()
//     {
//         await _postgresContainer.DisposeAsync();
//         await _mongoContainer.DisposeAsync();
//         await _redisContainer.DisposeAsync();
//     }
// }
