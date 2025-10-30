using ExpenseTracker.ExDbContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpenseTracker.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExpenseDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ExpenseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginAsync()
        {
            if (_transaction != null)
                return;
           _transaction =  await _dbContext.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No Transaction Started");
            await _dbContext.SaveChangesAsync();
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            if(_transaction is null)
             throw new InvalidOperationException("No Transaction Started");
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}