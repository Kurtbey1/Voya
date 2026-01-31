using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voya.Models;

namespace Voya.Data.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.User_ID);

            entity.Property(u => u.User_ID)
                  .ValueGeneratedNever();

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.Password_Hash)
                  .HasColumnType("nvarchar(max)")
                  .IsRequired();

            entity.Property(u => u.Role)
                  .HasMaxLength(20)
                  .HasDefaultValue("Customer");

            entity.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
