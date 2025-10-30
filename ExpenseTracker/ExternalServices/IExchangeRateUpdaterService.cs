namespace ExpenseTracker.ExternalServices
{
    public interface IExchangeRateUpdaterService
    {
        Task UpdateRateAsync();
    }
}