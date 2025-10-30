using ExpenseTracker.Configurations.DbInitializer;
using ExpenseTracker.Endpoints;
using ExpenseTracker.Extensions;
using ExpenseTracker.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using StackExchange.Redis;

// ------------------------------
// Application builder
// ------------------------------
var builder = WebApplication.CreateBuilder(args);

// ✅ Ensure environment variables override appsettings.json
builder.Configuration.AddEnvironmentVariables();

// ------------------------------
// Service registration
// ------------------------------
builder.Services.AddDatabaseRegistration(builder.Configuration);

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRepoRegistration();
builder.Services.AddHttpContextAccessor();
builder.Services.AddServicesRegistration();
builder.Services.AddServiceRegistration();
builder.Services.OtherServicesRegistration(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ------------------------------
// Logging
// ------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// ------------------------------
// Health Checks
// ------------------------------
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgres")
    .AddMongoDb(builder.Configuration.GetConnectionString("MongoDb")!, name: "mongodb")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis");

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------
// Endpoints
// ------------------------------
app.MapGet("/", () => Results.Ok(new
{
    message = "🚀 Expense Tracker API is up and running successfully!",
    version = "v1.0",
    status = "Running",
    timestamp = DateTime.UtcNow
}));

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapCategoryEndpoints();
app.MapExpenseEndpoints();
app.MapReportEndpoints();
app.MapActivityLogEndpoints();
app.MapHealthChecks("/health");

// ------------------------------
// Database initialization with retry
// ------------------------------
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var retryCount = 10;
var delay = TimeSpan.FromSeconds(10);

logger.LogInformation("Starting database initialization with up to {RetryCount} retries.", retryCount);

for (int i = 0; i < retryCount; i++)
{
    try
    {
        logger.LogInformation("Attempting to initialize the database... (Attempt {AttemptNumber})", i + 1);
        await DatabaseInitializer.InitializeAsync(app.Services);
        logger.LogInformation("✅ Database initialization successful.");
        break; // Success
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database initialization failed on attempt {Attempt}. Retrying in {DelaySeconds}s...", i + 1, delay.TotalSeconds);

        if (i == retryCount - 1)
        {
            logger.LogError("❌ Could not initialize the database after {RetryCount} attempts. Exiting application.", retryCount);
            throw; // Stop app startup
        }

        await Task.Delay(delay);
    }
}

// ------------------------------
// Signal application readiness
// ------------------------------
var startupState = app.Services.GetRequiredService<ApplicationStartupState>();
startupState.SetApplicationStarted();

logger.LogInformation("🚀 Expense Tracker API is now running and ready to accept requests.");

// ------------------------------
// Run the web host
// ------------------------------
await app.RunAsync();

public partial class Program { }
