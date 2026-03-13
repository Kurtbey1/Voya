using Voya.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voya.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> entity)
        {
            entity.ToTable("Hotels");

            entity.HasKey(h => h.Hotel_ID);

            entity.Property(h => h.Hotel_ID)
                  .ValueGeneratedNever();

            entity.Property(h => h.Hotel_Name)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(h => h.Description)
                  .IsRequired()
                  .HasMaxLength(2000);

            entity.Property(h => h.Address)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(h => h.BasePricePerNight)
                  .HasPrecision(18, 2)
                  .IsRequired();

            entity.Property(h => h.MainImageUrl)
                  .IsRequired()
                  .HasMaxLength(1000);

            entity.Property(h => h.StarRating)
                  .IsRequired();

            entity.ToTable(t => t.HasCheckConstraint("CHK_Hotel_StarRating", "[StarRating] >= 1 AND [StarRating] <= 5"));

            entity.HasMany(h => h.Bookings)
                  .WithOne(b => b.Hotel)
                  .HasForeignKey(b => b.Hotel_ID)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
