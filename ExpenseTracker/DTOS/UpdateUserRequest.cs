namespace ExpenseTracker.DTOS.User
{
    public class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public string? DefaultCurrency { get; set; }
    }
}