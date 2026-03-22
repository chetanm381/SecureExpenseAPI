using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureExpenseAPI.Entities;

namespace SecureExpenseAPI.Data.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            ;

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.HasOne(e => e.User)
    .WithMany(u => u.Expenses)
    .HasForeignKey(e => e.UserId)
    .OnDelete(DeleteBehavior.Cascade);
    }
}