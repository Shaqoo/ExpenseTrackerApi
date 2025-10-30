using ExpenseTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Configurations.EntityConfiguration
{
    public class KPIRecordEntityTypeConfig : IEntityTypeConfiguration<KPIRecord>
    {
        public void Configure(EntityTypeBuilder<KPIRecord> builder)
        {
            builder.ToTable("KPIRecords");

            builder.HasKey(k => k.Id);

            builder.Property(k => k.UserId)
                   .IsRequired();

            builder.Property(p => p.MetricName)
                   .IsRequired(true);
            
            builder.Property(p => p.Value)
                   .IsRequired();

        }
    }
}