using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;
namespace Voya.Data.Configurations
{
    public class TicketConfiguration:IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> entity)
        {
            entity.ToTable("Tickets");

            entity.Property(t => t.Ticket_ID)
                .ValueGeneratedNever();

            entity.Property(t => t.Ticket_Type)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(t => new { t.Flight_ID, t.Seat_Number })
             .IsUnique();

            entity.Property(t => t.Seat_Number)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(t => t.Flight)
                .WithMany(f => f.Tickets)
                .HasForeignKey(t => t.Flight_ID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.Booking_ID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
