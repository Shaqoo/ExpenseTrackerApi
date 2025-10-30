using ExpenseTracker.Configurations.EntityConfiguration;
using ExpenseTracker.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ExDbContext
{
    public class ExpenseDbContext : IdentityDbContext<User,IdentityRole<Guid>,Guid>
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        { }
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
        public DbSet<KPIRecord> KPIRecords => Set<KPIRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityTypeConfig).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}