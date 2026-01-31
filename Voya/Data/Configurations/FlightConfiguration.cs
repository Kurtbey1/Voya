using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;
namespace Voya.Data.Configurations
{
    public class FlightConfiguration:IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> entity)
        {
            entity.ToTable("Flights",table =>
            {
                table.HasCheckConstraint("CHK_ScheduledArrival_Is_After_ScheduledDeparture",
                    "[ScheduledArrival] >[ScheduledDeparture]");
            });

            entity.HasKey(f => f.Flight_ID);

            entity.Property(f => f.Plane)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(f => f.Source)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(f => f.Destination)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(f => f.ScheduledDeparture)
                .IsRequired()
                .HasColumnType("DATETIME2");

            entity.Property(f => f.ScheduledArrival)
                .IsRequired()
                .HasColumnType("DATETIME2");

            entity.Property(f => f.Flight_ID)
                .ValueGeneratedNever();

            entity.HasOne(f => f.Company)
                .WithMany(c => c.Flights)
                .HasForeignKey(f =>f.Company_ID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
