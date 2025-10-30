
using ExpenseTracker.ExternalServices;
using ExpenseTracker.Extensions;

namespace ExpenseTracker.BackgroundServices
{
    public class ExchangeRateUpdaterJob : BackgroundService
    {
        private readonly ILogger<ExchangeRateUpdaterJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationStartupState _startupState;

        public ExchangeRateUpdaterJob(ILogger<ExchangeRateUpdaterJob> logger, IServiceProvider serviceProvider, ApplicationStartupState startupState)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _startupState = startupState;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait for the application to be fully started
            await _startupState.ApplicationStarted;

            _logger.LogInformation("Exchange Rate Updater Job is starting.");

            // Run once at startup before the first delay.
            await DoWorkAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Next exchange rate update scheduled in 1 hour.");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await DoWorkAsync(stoppingToken);
                }
            }

            _logger.LogInformation("Exchange Rate Updater Job is stopping.");
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Exchange Rate Updater Job is running at: {time}", DateTimeOffset.Now);
            using var scope = _serviceProvider.CreateScope();
            var exchangeRateUpdaterService = scope.ServiceProvider.GetRequiredService<IExchangeRateUpdaterService>();
            await exchangeRateUpdaterService.UpdateRateAsync();
        }
    }
}