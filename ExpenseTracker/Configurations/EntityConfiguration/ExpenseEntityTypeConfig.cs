using ExpenseTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Configurations.EntityConfiguration
{
    public class ExpenseEntityTypeConfig : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.User)
                   .WithMany(u => u.Expenses)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Category)
                   .WithMany(c => c.Expenses)
                   .HasForeignKey(e => e.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(e => e.Currency)
                   .IsRequired()
                   .HasMaxLength(5)
                   .HasDefaultValue("USD");

            builder.Property(e => e.ConvertedAmountUSD)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(e => e.Type)
                   .IsRequired()
                   .HasConversion<int>();  

            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(e => e.Notes)
                   .HasMaxLength(500);

            builder.Property(e => e.IsRecurring)
                   .HasDefaultValue(false);

            builder.HasIndex(e => e.Date);
            builder.HasIndex(e => e.Type);
            builder.HasIndex(e => new { e.UserId, e.CategoryId });

        }
    }
}
