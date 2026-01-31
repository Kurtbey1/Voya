using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;

namespace Voya.Data.Configurations
{
    public class PaymentConfiguration:IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> entity)
        {
            entity.ToTable("Payments", table =>
            {
                table.HasCheckConstraint(
                    "CHK_Payments_Amount_Positive",
                    "[Amount] > 0"
                    );
            });

            entity.HasKey(p => p.Payment_ID);

            entity.Property(p => p.Payment_ID)
            .ValueGeneratedNever();

            entity.HasOne(p => p.Transaction)
           .WithOne(t => t.Payment)
           .HasForeignKey<Payment>(p => p.Transaction_ID)
           .OnDelete(DeleteBehavior.Restrict);

            entity.Property(p => p.Payment_Method)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Credit Card");

            entity.Property(p => p.Amount)
           .IsRequired()
           .HasColumnType("decimal(18,2)");
        }
    }
}
