namespace ExpenseTracker.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();    
    }
}