using Voya.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voya.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> entity)
        {
            entity.ToTable("Bookings", table =>
            {
                table.HasCheckConstraint(
                    "CHK_Bookings_Adults_Positive",
                    "[Adults_Number] >= 1"
                );

                table.HasCheckConstraint(
                    "CHK_Bookings_Children_NonNegative",
                    "[Children_Number] >= 0"
                );
            });

            entity.HasKey(b => b.Booking_ID);

            entity.Property(b => b.Booking_ID)
                  .ValueGeneratedNever();

            entity.Property(b => b.Booking_Date)
                  .IsRequired()
                  .HasColumnType("DATETIME2");

            entity.Property(b => b.Booking_State)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(b => b.Adults_Number)
                  .IsRequired();

            entity.Property(b => b.Children_Number)
                  .IsRequired();

            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(b => b.User_ID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(b => b.Transactions)
                  .WithOne(t => t.Booking)
                  .HasForeignKey(t => t.Booking_ID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(b => b.RoomReservations)
                  .WithOne(r => r.Booking)
                  .HasForeignKey(r => r.Booking_ID)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}