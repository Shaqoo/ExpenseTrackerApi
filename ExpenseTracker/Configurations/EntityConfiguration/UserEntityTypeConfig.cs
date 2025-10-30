using ExpenseTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Configurations.EntityConfiguration
{
    public class UserEntityTypeConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(a => a.Id);

            builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.PasswordHash)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(a => a.DefaultCurrency)
                   .HasMaxLength(5)
                   .HasDefaultValue("USD");

            builder.Property(a => a.CreatedAt);

            builder.HasIndex(a => a.Email)
                    .IsUnique(true);

            builder.HasMany(a => a.Categories)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Expenses)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}