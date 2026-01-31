using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;

namespace Voya.Data.Configurations
{
    public class Airline_CompanyConfiguration:IEntityTypeConfiguration<Airline_Company>
    {
        public void Configure(EntityTypeBuilder<Airline_Company> entity)
        {
            entity.ToTable("Airline_Companies",table =>
            {
                table.HasCheckConstraint("CHK_AirlineCompanies_Rate_Valid",
                    "[Rate] >= 0 AND [Rate] <= 5");
            });

            entity.HasKey(c => c.Company_ID);

            entity.Property(c => c.Company_ID)
                .ValueGeneratedNever();

            entity.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.Rate)
                .IsRequired()
                .HasColumnType("decimal(3,2)");

            entity.HasMany(c=>c.Flights)
                .WithOne(f => f.Company)
                .HasForeignKey(f => f.Company_ID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
