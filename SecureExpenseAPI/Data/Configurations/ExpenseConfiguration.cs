using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using SecureExpenseAPI.Entities;  


namespace SecureExpenseAPI.Data.Configurations;

    

public class IExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);

        // Title Configuration
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(255);

        // Amount Configuration
        builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Date Configuration
        builder.Property(e => e.Date)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);

        // UserId Configuration
        builder.Property(e => e.UserId)
            .IsRequired();

        // Relationship Configuration
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}