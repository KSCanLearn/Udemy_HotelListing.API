using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations
{
    public class RolesConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "3001",
                    Name = "Adminstrator",
                    NormalizedName = "ADMINSTRATOR"
                },
                new IdentityRole
                {
                    Id = "3002",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
        }
    }
}
