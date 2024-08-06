using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{
    public class HotelListingDbContext : DbContext
    {
        public HotelListingDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(
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

            modelBuilder.Entity<Hotel>().HasData(
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
