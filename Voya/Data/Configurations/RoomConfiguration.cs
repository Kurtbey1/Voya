using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;

namespace Voya.Data.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> entity)
        {
            entity.ToTable("Rooms", table =>
            {
                table.HasCheckConstraint(
                    "CHK_Rooms_Price_Per_Night_Positive",
                    "[Price_Per_Night] > 0"
                );
            });

            entity.HasKey(r => r.Room_ID);

            entity.Property(r => r.Room_ID)
                  .ValueGeneratedNever();

            entity.Property(r => r.Base_Capacity)
                  .IsRequired();

            entity.Property(r => r.Price_Per_Night)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            entity.HasOne(r => r.Hotel)
                  .WithMany(h => h.Rooms)
                  .HasForeignKey(r => r.Hotel_ID)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
