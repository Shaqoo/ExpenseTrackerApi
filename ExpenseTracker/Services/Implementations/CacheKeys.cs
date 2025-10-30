namespace ExpenseTracker.Services.Helpers
{
    public static class CacheKeys
    {
        public static string Categories(Guid userId) => $"categories_{userId}";
        public static string Expenses(Guid userId) => $"expenses_{userId}";
    }
}