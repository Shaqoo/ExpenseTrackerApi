using ExpenseTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Configurations.EntityConfiguration
{
    public class CategoryEntityTypeConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                   .IsRequired();

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Description)
                   .HasMaxLength(255);

            builder.HasIndex(c => new { c.UserId, c.Name })
                   .IsUnique();

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Categories)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}