using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;

namespace Voya.Data.Configurations
{

    public class RoomReservationConfiguration
    : IEntityTypeConfiguration<RoomReservation>
    {
        public void Configure(EntityTypeBuilder<RoomReservation> entity)
        {
            entity.ToTable("RoomReservations", table =>
            {
                table.HasCheckConstraint(
                    "CHK_RoomReservations_CheckOut_After_CheckIn",
                    "[Check_Out] > [Check_In]"
                );
            });

            entity.HasKey(r => r.Reservation_ID);

            entity.Property(r => r.Reservation_ID)
                  .ValueGeneratedNever();

            entity.Property(r => r.Check_In)
                  .IsRequired()
                  .HasColumnType("DATETIME2");

            entity.Property(r => r.Check_Out)
                  .IsRequired()
                  .HasColumnType("DATETIME2");

            entity.HasOne(r => r.Room)
                  .WithMany(room => room.RoomReservations)
                  .HasForeignKey(r => r.Room_ID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Booking)
                  .WithMany(b => b.RoomReservations)
                  .HasForeignKey(r => r.Booking_ID)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}