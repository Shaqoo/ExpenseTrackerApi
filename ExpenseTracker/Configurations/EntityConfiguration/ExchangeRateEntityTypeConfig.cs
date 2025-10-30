using ExpenseTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Configurations.EntityConfiguration
{
    public class ExchangeRateEntityTypeConfig : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.ToTable("ExchangeRates");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.BaseCurrency)
                   .IsRequired()
                   .HasMaxLength(5);

            builder.Property(e => e.TargetCurrency)
                   .IsRequired()
                   .HasMaxLength(5);

            builder.Property(e => e.Rate)
                   .IsRequired()
                   .HasColumnType("decimal(18,6)");

            builder.Property(e => e.RetrievedAt)
                   .IsRequired();

            builder.Property(e => e.Source)
                   .HasMaxLength(100)
                   .HasDefaultValue("OpenExchangeRates");

            builder.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency })
                   .IsUnique();
          
            builder.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency, e.RetrievedAt });
        }
    }
}
