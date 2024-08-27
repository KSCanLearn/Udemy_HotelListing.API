using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
                new Country
                {
                    Id = 1001,
                    Name = "Malaysia",
                    ShortName = "MY",
                },
                new Country
                {
                    Id = 1002,
                    Name = "Singapore",
                    ShortName = "SG",
                }
            );
        }
    }
}
