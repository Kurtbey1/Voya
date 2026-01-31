using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;

namespace Voya.Data.Configurations
{
    public class TransactionConfiguration:IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> entity)
        {

            entity.ToTable("Transactions");

            entity.HasKey(t => t.Transaction_ID);

            entity.HasOne(t => t.Booking)
            .WithMany(b => b.Transactions)
            .HasForeignKey(t => t.Booking_ID)
            .OnDelete(DeleteBehavior.Restrict);

            entity.Property(t => t.Transaction_ID)
            .ValueGeneratedNever();

            entity.Property(t => t.Transaction_Type)
            .IsRequired()
            .HasMaxLength(50);

            entity.Property(t => t.Transaction_Status)
            .IsRequired()
            .HasMaxLength(50);

            entity.Property(t => t.Transaction_Date)
            .IsRequired()
            .HasColumnType("DATETIME2");

            entity.Property(t => t.Provider_Ref)
            .IsRequired()
            .HasMaxLength(255);
            
        }
    }
}
