using Voya.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voya.Data.Configurations
{
    public class HotelConfiguration: IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel>entity)
        {
            entity.ToTable("Hotels");

            entity.HasKey(h => h.Hotel_ID);

            entity.Property(h => h.Hotel_ID)
            .ValueGeneratedNever();

            entity.HasMany(h => h.Rooms)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.Hotel_ID)
            .OnDelete(DeleteBehavior.Restrict);


            entity.Property(h => h.Hotel_Name)
             .IsRequired()
             .HasMaxLength(50);
        }
    }
}
