namespace ExpenseTracker.DTOS
{
    public class ActivityLogDto
    {
        public string? Id { get; set; }
        public Guid? UserId { get; set; }
        public required string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}