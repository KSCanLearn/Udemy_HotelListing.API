using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    Id = 2001,
                    Name = "Hilton Hotel Malaysia",
                    Address = "Petaling Jaya",
                    CountryId = 1001,
                    Rating = 4.2
                },
                new Hotel
                {
                    Id = 2002,
                    Name = "Hilton Hotel Singapore",
                    Address = "Clarke Clay",
                    CountryId = 1002,
                    Rating = 4
                }
            );
        }
    }
}
