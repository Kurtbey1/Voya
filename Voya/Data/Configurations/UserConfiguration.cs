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

            entity.Property(u => u.User_Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.HasIndex(u => u.Email)
                  .IsUnique();
            entity.ToTable(t => t.HasCheckConstraint("CK_User_Email", "Email LIKE '%_@__%.__%'"));

            entity.Property(u => u.Password_Hash)
                  .HasMaxLength(256)
                  .IsRequired();

            entity.Property(u => u.Phone)
                  .HasMaxLength(20)
                  .IsRequired();

            entity.Property(u => u.Nationality)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(u => u.IsActive)
                   .IsRequired()
                   .HasDefaultValue(false);

            entity.Property(u => u.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            entity.Property(u => u.Role)
                  .HasMaxLength(20)
                  .HasDefaultValue("Customer");

            entity.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
